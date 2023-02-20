using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.AdvRedirect.Domain;
using Nop.Plugin.Misc.AdvRedirect.Models.Redirections;

namespace Nop.Plugin.Misc.AdvRedirect.Infrastructure.Mapper
{
    /// <summary>
    /// Represents AutoMapper configuration for plugin models
    /// </summary>
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public MapperConfiguration()
        {
            CreateMap<RedirectionModel, RedirectionRule>();
            CreateMap<RedirectionRule, RedirectionModel>();

			CreateMap<RedirectionCSVModel, RedirectionModel>();
			CreateMap<RedirectionRule, RedirectionCSVModel>();
		}

        #endregion

        #region Properties

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 1;

        #endregion
    }
}