﻿using Microsoft.SqlServer.Dac.Extensions.Prototype;
using Microsoft.SqlServer.Dac.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReverseEngineer20
{
    using EFCorePowerTools.Shared.Models;

    public class DacpacTableListBuilder
    {
        private readonly string _dacpacPath;

        public DacpacTableListBuilder(string dacpacPath)
        {
            if (string.IsNullOrEmpty(dacpacPath))
            {
                throw new ArgumentException(@"invalid path", nameof(dacpacPath));
            }
            if (!File.Exists(dacpacPath))
            {
                throw new ArgumentException("Dacpac file not found");
            }
            _dacpacPath = dacpacPath;
        }

        public List<TableInformationModel> GetTableDefinitions(bool includeViews = false)
        {
            var model = new TSqlTypedModel(_dacpacPath);
            var result = model.GetObjects<TSqlTable>(DacQueryScopes.UserDefined)
                        .Select(m => new TableInformationModel(m.Name.Parts[0] + "." + m.Name.Parts[1], m.PrimaryKeyConstraints.Any()))
                        .ToList();

            if (includeViews)
            {
                var views = model.GetObjects<TSqlView>(DacQueryScopes.UserDefined)
                            .Select(m => new TableInformationModel(m.Name.Parts[0] + "." + m.Name.Parts[1], false))
                            .ToList();
                result = result.Concat(views).ToList();
            }

            return result;
        }
    }
}
