﻿using System.Collections.Immutable;
using AutoMapper;

namespace FloppyBot.Commands.Custom.Storage.Entities.Internal;

public class CustomCommandStorageProfile : Profile
{
    public CustomCommandStorageProfile()
    {
        // basic type conversions
        CreateMap<IImmutableSet<string>, string[]>()
            .ConvertUsing<StringSetAndArrayConverter>();
        CreateMap<string[], IImmutableSet<string>>()
            .ConvertUsing<StringSetAndArrayConverter>();

        // dto -> eo
        CreateMap<CustomCommandDescription, CustomCommandDescriptionEo>()
            .ForMember(
                e => e.Aliases,
                o => o.Ignore())
            .AfterMap((dto, eo) =>
            {
                eo.Aliases = dto.Aliases.AsEnumerable()
                    .OrderBy(i => i)
                    .ToArray();
            });
        CreateMap<CommandResponse, CommandResponseEo>();
        CreateMap<CommandLimitation, CommandLimitationEo>();
        CreateMap<CooldownDescription, CooldownDescriptionEo>();

        // eo -> dto
        CreateMap<CustomCommandDescriptionEo, CustomCommandDescription>()
            .ConvertUsing((eo, _, ctx) => new CustomCommandDescription
            {
                Id = eo.Id,
                Name = eo.Name,
                Aliases = eo.Aliases.ToImmutableHashSet(),
                Responses = eo.Responses
                    .Select(e => ctx.Mapper.Map<CommandResponse>(e))
                    .ToImmutableList(),
                Limitations = ctx.Mapper.Map<CommandLimitation>(eo.Limitations),
            });
        CreateMap<CommandResponseEo, CommandResponse>();
        CreateMap<CommandLimitationEo, CommandLimitation>()
            .ConvertUsing((eo, _, ctx) => new CommandLimitation
            {
                MinLevel = eo.MinLevel,
                Cooldown = eo.Cooldown
                    .Select(c => ctx.Mapper.Map<CooldownDescription>(c))
                    .ToImmutableHashSet()
            });
        CreateMap<CooldownDescriptionEo, CooldownDescription>();
    }
}
