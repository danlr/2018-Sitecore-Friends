using System;
using System.Collections.Generic;
using System.Data;
using Hackathon.XEditor.Api.Services;
using Sitecore.Cintel.Reporting;
using Sitecore.Cintel.Reporting.Processors;
using System.Linq;
using System.Threading.Tasks;

namespace Hackathon.XEditor.Api.Pipelines
{
    public  class PopulateWithFacetData: ReportProcessorBase
    {
        private static readonly Type BaseType = typeof(Sitecore.XConnect.Facet);
        readonly List<string> _skippedProperties = BaseType.GetProperties().Select(x => x.Name).ToList();
        private const int maxDepth = 3;
        private bool IsDefaultProperty(string propertyName)
        {
                return _skippedProperties.Contains(propertyName);
        }

        public override void Process(ReportProcessorArgs args)
        {
            var xConnectService = new XconnectService();
            var facets = Task.Run(() => xConnectService.GetFacets(args.ReportParameters.ContactId)).Result;
            var table = CreateDataTable(facets); 
            args.ResultSet.Data.Dataset[args.ReportParameters.ViewName] = table;
        }

        public DataTable CreateDataTable(IEnumerable<dynamic> list)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add(new DataColumn("FacetName", "".GetType()));
            dataTable.Columns.Add(new DataColumn("FieldName", "".GetType()));
            dataTable.Columns.Add(new DataColumn("FieldValue", "".GetType()));
            dataTable.Columns.Add(new DataColumn("DepthLevel", "".GetType()));
            dataTable.Columns.Add(new DataColumn("Type", "".GetType()));
            foreach (var entity in list)
            {
                AddChildren(ref dataTable, entity,0);
            }

            return dataTable;
        }

        private void AddChildren(ref DataTable dataTable, dynamic obj, int depth, string typePath="")
        {
            if (depth > maxDepth) return; 
            Type type = obj.GetType();
            var properties = type.GetProperties();
            foreach (var p in properties)
            {
                object[] values = new object[5];

                values[0] = type.Name;
                values[1] = p.Name;
                var value = p.GetValue(obj) ?? "";
                values[2] = value;
                values[3] = depth;
              
                Type valueType = value.GetType();
                if (valueType.FullName != null)
                {
                    if (!valueType.FullName.StartsWith("System.Collections"))
                        if (!IsDefaultProperty(p.Name))
                        {
                            var path = typePath+ "$" + values[1];
                            values[4] = typePath;
                            if (valueType.Namespace.StartsWith("System"))
                            {
                                dataTable.Rows.Add(values);
                            }
                            else
                            {
                                values[2] = "$object$";
                                dataTable.Rows.Add(values);
                                AddChildren(ref dataTable, value, depth+1, path);
                            }
                        }
                }
            }
        }

    }
}
