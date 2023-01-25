using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.CodeInjector.Models;
using Nop.Plugin.Misc.CodeInjector.Services;

namespace Nop.Plugin.Misc.CodeInjector.Infrastructure.Mapper
{
    /// <summary>
    /// Represents AutoMapper configuration for plugin models
    /// </summary>
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public MapperConfiguration()
        {
            CreateMap<CodeToInject, CodeToInjectDTO>();
            CreateMap<CodeToInjectDTO, CodeToInject>();
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