using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class GenerationMapper : IGenerationMapper
    {
        readonly IAssetMapper _assetMapper;
        readonly ICarConfiguratorVersionMapper _carConfiguratorVersionMapper;
        readonly ILabelMapper _labelMapper;
        readonly ILinkMapper _linkMapper;

        public GenerationMapper(IAssetMapper assetMapper, ICarConfiguratorVersionMapper carConfiguratorVersionMapper, ILabelMapper labelMapper, ILinkMapper linkMapper)
        {
            if (assetMapper == null) throw new ArgumentNullException("assetMapper");
            if (carConfiguratorVersionMapper == null) throw new ArgumentNullException("carConfiguratorVersionMapper");
            if (labelMapper == null) throw new ArgumentNullException("labelMapper");
            if (linkMapper == null) throw new ArgumentNullException("linkMapper");

            _assetMapper = assetMapper;
            _carConfiguratorVersionMapper = carConfiguratorVersionMapper;
            _labelMapper = labelMapper;
            _linkMapper = linkMapper;
        }

        public Generation MapGeneration(Administration.Model model, Administration.ModelGeneration generation, String brand, String country, String language, Boolean isPreview)
        {
            return new Generation
            {
                Assets = generation.Assets.Select(_assetMapper.MapLinkedAsset).ToList(),
                CarConfiguratorVersion = _carConfiguratorVersionMapper.MapCarConfiguratorVersion(generation.ActiveCarConfiguratorVersion),
                Description = generation.Translation.Description,
                FootNote = generation.Translation.FootNote,
                ID = generation.ID,
                InternalCode = generation.BaseCode,
                Labels = generation.Translation.Labels.Select(_labelMapper.MapLabel).ToList(),
                Links = model.Links.Where(link => IsApplicableLink(link, generation))
                                   .Select(link => _linkMapper.MapLink(link, country, language, isPreview))
                                   .ToList(),
                LocalCode = generation.LocalCode.DefaultIfEmpty(generation.BaseCode), //String.IsNullOrWhiteSpace(generation.LocalCode) ? generation.BaseCode : generation.LocalCode,
                Name = generation.Translation.Name.DefaultIfEmpty(generation.Name),
                SortIndex = model.Index,
                SSN = generation.FactoryGenerations.First().SSN,
                ToolTip = generation.Translation.ToolTip,
                
            };
        }

        private static Boolean IsApplicableLink(Administration.Link link, Administration.ModelGeneration generation)
        {
            return link.Type.CarConfiguratorversionID == generation.ActiveCarConfiguratorVersion.ID ||
                   link.Type.CarConfiguratorversionID == 0;
        }
    }
}
