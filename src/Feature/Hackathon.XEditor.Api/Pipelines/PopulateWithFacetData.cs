using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Hackathon.XEditor.Api.Dto;
using Hackathon.XEditor.Api.Services;
using Hackathon.XEditor.Model.Contact.Facets;
using Sitecore.Cintel.Reporting;
using Sitecore.Cintel.Reporting.Processors;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

namespace Hackathon.XEditor.Api.Pipelines
{
    public  class PopulateWithFacetData: ReportProcessorBase
    {
        public override void Process(ReportProcessorArgs args)
        {
            var xConnectService = new XconnectService();
            var facets = Task.Run(() => xConnectService.GetFacets(args.ReportParameters.ContactId)).Result;
            var table = CreateDataTable(facets); 
            args.ResultSet.Data.Dataset[args.ReportParameters.ViewName] = table;
        }

        public static DataTable CreateDataTable(IEnumerable<dynamic> list)
        {
            Type baseType = typeof(Sitecore.XConnect.Facet);
            var skippedProperties = baseType.GetProperties().Select(x => x.Name).ToList();

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add(new DataColumn("FacetName", "".GetType()));
            dataTable.Columns.Add(new DataColumn("FieldName", "".GetType()));
            dataTable.Columns.Add(new DataColumn("FieldValue", "".GetType()));

            foreach (var entity in list)
            {
                Type type = entity.GetType();
                var properties = type.GetProperties();
                foreach (var p in properties)
                {
                    object[] values = new object[3];

                    values[0] = type.Name;
                    values[1] = p.Name;
                    var value = p.GetValue(entity) ?? "";
                    values[2] = value;
                    Type valueType = value.GetType();
                    if (valueType.FullName != null)
                    {
                        if (!valueType.FullName.StartsWith("System.Collections"))
                            if (!skippedProperties.Contains(p.Name))
                                dataTable.Rows.Add(values);
                    }
                   

                }
               
            }

            return dataTable;
        }
    }
}
