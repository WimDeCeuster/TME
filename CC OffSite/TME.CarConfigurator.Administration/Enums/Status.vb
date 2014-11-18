Namespace Enums
    <Serializable()> Friend Enum Status
        AvailableToNmscs = 1
        ApprovedForLive = 2
        ApprovedForPreview = 4
        Declined = 8
        Deleted = 16  'Used for deleting/hiding global items (currently only implemeted for StandardCar)
        Special = 32 'Only used to indicate non-special grades
        Promoted = 128

        Importing = 256
        Normalizing = 512
        BluePrinting = 1024
        Copying = 2048

    End Enum
End NameSpace