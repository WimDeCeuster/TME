using System;
using System.Linq;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class GenerationMapper : IGenerationMapper
    {
        readonly IAssetMapper _assetMapper;
        readonly ICarConfiguratorVersionMapper _carConfiguratorVersionMapper;
        readonly IBaseMapper _baseMapper;
        readonly ILinkMapper _linkMapper;

        public GenerationMapper(IAssetMapper assetMapper, ICarConfiguratorVersionMapper carConfiguratorVersionMapper, IBaseMapper baseMapper, ILinkMapper linkMapper)
        {
            if (assetMapper == null) throw new ArgumentNullException("assetMapper");
            if (carConfiguratorVersionMapper == null) throw new ArgumentNullException("carConfiguratorVersionMapper");
            if (baseMapper == null) throw new ArgumentNullException("baseMapper");
            if (linkMapper == null) throw new ArgumentNullException("linkMapper");

            _assetMapper = assetMapper;
            _carConfiguratorVersionMapper = carConfiguratorVersionMapper;
            _baseMapper = baseMapper;
            _linkMapper = linkMapper;
        }

        public Generation MapGeneration(Administration.Model model, Administration.ModelGeneration generation, Boolean isPreview)
        {
            var mappedGeneration = new Generation
            {
                Assets = generation.Assets.Select(_assetMapper.MapLinkedAsset).ToList(),
                CarConfiguratorVersion = _carConfiguratorVersionMapper.MapCarConfiguratorVersion(generation.ActiveCarConfiguratorVersion),
                Links = model.Links.Where(link => link.IsApplicableLink(generation))
                                   .Select(link => _linkMapper.MapLink(link, isPreview))
                                   .ToList(),
                SortIndex = model.Index,
                SSN = generation.FactoryGenerations.First().SSN                
            };

            return _baseMapper.MapDefaults(mappedGeneration, generation, generation, generation.Name);
        }
    }
}
