using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Hackathon.XEditor.Api.Dto;
using Hackathon.XEditor.Model.Contact.Facets;
using Sitecore.Cintel.Reporting;
using Sitecore.Cintel.Reporting.Processors;

namespace Hackathon.XEditor.Api.Pipelines
{
    public  class PopulateWithFacetData: ReportProcessorBase
    {
        public override void Process(ReportProcessorArgs args)
        {
            var contact = new ContactDto
            {
                Phone = "+213213",
                Email = "x32@gmail.com",
                ContactId = args.ReportParameters.ContactId.ToString("N"),
                PersonalInformation = null
            };

            var contact2 = new ContactDto
            {
                Phone = "+22222222222",
                Email = "4444444@gmail.com",
                ContactId = args.ReportParameters.ContactId.ToString("N"),
                PersonalInformation = null
            };

            var table = CreateDataTable(new List<ContactDto> { contact, contact2 }); //contact.Result;
            args.ResultSet.Data.Dataset[args.ReportParameters.ViewName] = table;
        }

        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
