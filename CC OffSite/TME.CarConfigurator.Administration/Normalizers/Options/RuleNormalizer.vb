Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

Namespace Normalizers.Options
    Public Class RuleNormalizer
        Implements IGenerationNormalizer

        Public Sub Normalize(ByVal modelGeneration As ModelGeneration, ByVal allCars As Boolean) Implements IGenerationNormalizer.Normalize
            Dim cars = modelGeneration.Cars.Where(Function(c) allCars OrElse c.Approved OrElse c.Preview).ToList()
            Dim options = modelGeneration.Equipment.OfType(Of ModelGenerationOption)()

            For Each modelGenerationOption In options
                Dim carOptions = GetCarOptionsForModelGenerationOption(cars, modelGenerationOption).ToList()
                If carOptions.Any() Then
                    NormalizeOptionRules(modelGenerationOption, carOptions)
                End If

                If Not modelGenerationOption.IsValid Then
                    Throw New ApplicationException(String.Format("The option {0} is not valid : {1}", modelGenerationOption.Name, modelGenerationOption.BrokenRulesCollection.ToString()))
                End If
            Next
        End Sub

        Private Shared Sub NormalizeOptionRules(ByVal modelGenerationOption As ModelGenerationOption, ByVal carOptions As List(Of CarOption))
            Dim allRuleIds = GetAllRuleIds(modelGenerationOption, carOptions).ToList()

            
            For Each ruleId In allRuleIds
                Dim carOptionsThatHaveTheRuleAsInclude = GetCarOptionsThatHaveTheRule(carOptions, ruleId, RuleType.Include).ToList()
                Dim carOptionsThatHaveTheRuleAsExclude = GetCarOptionsThatHaveTheRule(carOptions, ruleId, RuleType.Exclude).ToList()
                Dim carOptionsThatDoNotHaveTheRule = GetCarOptionsThatDoNotHaveTheRule(carOptions, ruleId).ToList()

                '--------------------------------------------------------------
                'order of importance : INCLUDE rules > EXCLUDE rules > NO rules
                'e.g. if INCLUDE == NO, include rules will be made
                '--------------------------------------------------------------

                'if most car options don't have the rule, remove the rule on generation level (if it exists)
                If carOptionsThatDoNotHaveTheRule.Count > Math.Max(carOptionsThatHaveTheRuleAsInclude.Count, carOptionsThatHaveTheRuleAsExclude.Count) Then
                    If modelGenerationOption.Rules.Contains(ruleId) Then
                        Dim modelGenerationRuleType = modelGenerationOption.Rules(ruleId).Type
                        modelGenerationOption.Rules.Remove(ruleId)

                        'add the rule back to all cars that need it
                        For Each caroption In If(modelGenerationRuleType = RuleType.Include, carOptionsThatHaveTheRuleAsInclude, carOptionsThatHaveTheRuleAsExclude).Where(Function(x) x.Rules.Contains(ruleId))
                            If caroption.Car.Equipment.Contains(ruleId) Then
                                caroption.Rules.Add(DirectCast(caroption.Car.Equipment(ruleId), CarOption), modelGenerationRuleType)
                            End If
                        Next

                    End If
                    Continue For
                End If

                'otherwise, the rule should be added (or modified) on generation level

                'check the rule type it should be on generation level
                Dim ruleTypeOnGenerationLevel = If(carOptionsThatHaveTheRuleAsInclude.Count >= carOptionsThatHaveTheRuleAsExclude.Count, RuleType.Include, RuleType.Exclude)
                'find the rule category it should be on generation level
                Dim ruleCategoryOnGenerationLevel = FindRuleCategoryForGenerationLevel(carOptions, ruleId)
                'find the rule colouringmodes it should be on generation level
                Dim ruleColouringModesOnGenerationLevel = FindRuleColouringModesForGenerationLevel(carOptions, ruleId, ruleTypeOnGenerationLevel)
                'try to find the modelGenerationRule
                Dim modelGenerationRule = modelGenerationOption.Rules(ruleId)

                'if it doesn't exist yet, add it
                If modelGenerationRule Is Nothing Then
                    modelGenerationRule = modelGenerationOption.Rules.Add(ruleId, ruleTypeOnGenerationLevel, ruleColouringModesOnGenerationLevel)

                    'remove the rule from all cars that do notneed it
                    For Each caroption In carOptionsThatDoNotHaveTheRule.Where(Function(x) x.Rules.Contains(ruleId))
                        caroption.Rules.Remove(ruleId)
                    Next

                Else 'if it already exists, make sure it's correct
                    modelGenerationRule.Type = ruleTypeOnGenerationLevel
                End If

                'and make the category correct in both cases
                modelGenerationRule.Category = ruleCategoryOnGenerationLevel
            Next
        End Sub

        

#Region "Helpers"
        Private Shared Function GetCarOptionsForModelGenerationOption(ByVal cars As IEnumerable(Of Car), ByVal modelGenerationOption As ModelGenerationOption) As IEnumerable(Of CarOption)
            Return From car In cars
                   Where car.Equipment.Contains(modelGenerationOption.ID)
                   Select DirectCast(car.Equipment(modelGenerationOption.ID), CarOption)
        End Function

        Private Shared Function GetAllRuleIds(ByVal modelGenerationOption As ModelGenerationOption, ByVal carOptions As IEnumerable(Of CarOption)) As IEnumerable(Of Guid)
            'work with ids => union of mgo and co is possible (see last line)
            Dim modelGenerationRuleIds = From rule In modelGenerationOption.Rules Select rule.ID
            Dim carRuleIds = (From carOption In carOptions
                                From rule In carOption.Rules
                                Select rule.ID)
            Return modelGenerationRuleIds.Union(carRuleIds)
        End Function

        Private Shared Function GetCarOptionsThatHaveTheRule(ByVal carOptions As IEnumerable(Of CarOption), ByVal ruleId As Guid, ByVal ruleType As RuleType) As IEnumerable(Of CarOption)
            Return carOptions.Where(Function(co) co.Rules.Contains(ruleId) AndAlso Not co.Rules(ruleId).Cleared AndAlso co.Rules(ruleId).RuleType = ruleType)
        End Function

        Private Shared Function GetCarOptionsThatDoNotHaveTheRule(ByVal carOptions As IEnumerable(Of CarOption), ByVal ruleId As Guid) As IEnumerable(Of CarOption)
            Return carOptions.Where(Function(co) Not co.Rules.Contains(ruleId) OrElse co.Rules(ruleId).Cleared)
        End Function

        Private Shared Function FindRuleCategoryForGenerationLevel(ByVal carOptions As List(Of CarOption), ByVal ruleId As Guid) As RuleCategory
            'PRODUCT > MARKETING > VISUAL
            If AtLeastOneCarOptionHasTheRuleDefinedAsTheGivenRuleCategory(carOptions, ruleId, RuleCategory.PRODUCT) Then Return RuleCategory.PRODUCT
            If AtLeastOneCarOptionHasTheRuleDefinedAsTheGivenRuleCategory(carOptions, ruleId, RuleCategory.MARKETING) Then Return RuleCategory.MARKETING
            Return RuleCategory.VISUAL
        End Function

        Private Shared Function AtLeastOneCarOptionHasTheRuleDefinedAsTheGivenRuleCategory(ByVal carOptions As IEnumerable(Of CarOption), ByVal ruleId As Guid, ByVal ruleCategory As RuleCategory) As Boolean
            Return carOptions.Any(Function(co) co.Rules(ruleId) IsNot Nothing AndAlso co.Rules(ruleId).Category = ruleCategory)
        End Function

        Private Shared Function FindRuleColouringModesForGenerationLevel(ByVal carOptions As IEnumerable(Of CarOption), ByVal ruleId As Guid, ByVal ruleType As RuleType) As ColouringModes
            Dim rule = GetCarOptionsThatHaveTheRule(carOptions, ruleId, ruleType).First().Rules(ruleId)
            If TypeOf rule Is CarEquipmentItemRule Then
                Return DirectCast(rule, CarEquipmentItemRule).ColouringMode
            Else
                Return ColouringModes.None
            End If
        End Function
#End Region
    End Class
End Namespace

