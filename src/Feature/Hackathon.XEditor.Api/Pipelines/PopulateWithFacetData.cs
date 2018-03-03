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
            var length = 0;
            var facets = Task.Run(() => xConnectService.GetContactFacets(args.ReportParameters.ContactId)).Result;

            var output = facets.Select(a => a.Value); 
             var table = CreateDataTable(output, length); 
            args.ResultSet.Data.Dataset[args.ReportParameters.ViewName] = table;
        }

        public static DataTable CreateDataTable(IEnumerable<Sitecore.XConnect.Facet> list, int length)
        {
            //Type type = typeof(Sitecore.XConnect.Facet);
            //var properties = type.GetProperties();

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("FieldName", "".GetType());
            dataTable.Columns.Add("FieldValue", "".GetType());


            //foreach (PropertyInfo info in properties.Where(x => !x.Name.Equals("XObject",StringComparison.OrdinalIgnoreCase)))
            //{
            //    dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            //}

            foreach (var entity in list)
            {
                Type type = typeof(Sitecore.XConnect.Facet);
                var properties = type.GetProperties();
                foreach (var p in properties)
                {
                   
                    for (int i = 0; i < properties.Length; i++)
                    {
                       
                        object[] values = new object[2];

                        values[0] = properties[i].Name;
                        values[1] = properties[i].GetValue(entity);
                        if (properties[i].Name.Contains("XObject"))
                            dataTable.Rows.Add(values);
                    }

                    
                }
               
            }

            return dataTable;
        }
    }
}
