// Import necessary namespaces
using System;
using System.IO;
using System.Web;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using VI.Base;
using VI.DB;
using VI.DB.Entities;
using QBM.CompositionApi.ApiManager;
using QBM.CompositionApi.Definition;
using QBM.CompositionApi.Crud;
using QER.CompositionApi.Portal;
using VI.DB.DataAccess;
using VI.DB.Sync;
using System.Runtime.ConstrainedExecution;

namespace QBM.CompositionApi
{
    // The PostPredefinedSQLAllColumns class implements the IApiProviderFor interface for the PortalApiProject
    public class api4PredefinedSQL : IApiProviderFor<QER.CompositionApi.Portal.PortalApiProject>,IApiProvider
    {
        // The Build method is where we define our API methods using the IApiBuilder
        public void Build(IApiBuilder builder)
        {
            // Add a POST method named "example/predefinedsql/allcolumns" to the API
            builder.AddMethod(Method.Define("example/predefinedsql/api4")
                  .Handle<PostedSQL, List<List<ColumnData>>>("POST", async (posted, qr, ct) =>
                  {
                      // Retrieve the UID of the currently logged-in user from the session
                      var strUID_Person = qr.Session.User().Uid;

                      // Initialize a list to hold the results, each result is a list of ColumnData objects (rows)
                      var results = new List<List<ColumnData>>();

                      // Resolve an instance of IStatementRunner from the session to execute SQL statements
                      var runner = qr.Session.Resolve<IStatementRunner>();

                      // Execute a predefined SQL statement with parameters
                      using (var reader = runner.SqlExecute(posted.IdentQBMLimitedSQL, new[]
                      {
                          // Pass parameters to the SQL query
                          QueryParameter.Create("UID_PersonHead", posted.uid_head),
                          QueryParameter.Create("UID_Department", posted.uid_department),
                          QueryParameter.Create("UserInserted", posted.userInserted)

                      }))
                      {
                          // Read each row returned by the SQL query
                          while (reader.Read())
                          {
                              // Initialize a list to hold the columns for the current row
                              var row = new List<ColumnData>();

                              // Loop through each field (column) in the current row
                              for (int i = 0; i < reader.FieldCount; i++)
                              {
                                  // Add the column name and value to the row
                                  row.Add(new ColumnData
                                  {
                                      Column = reader.GetName(i),
                                      Value = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString()
                                  });
                              }
                              // Add the row to the results list
                              results.Add(row);
                          }
                      }
                      // Return the results as a list of lists of ColumnData objects
                      return results;
                  }));
        }

        // The ColumnData class represents a single column and its value in a database row
        public class ColumnData
        {
            // The name of the column
            public string Column { get; set; }
            // The value of the column in the current row
            public string Value { get; set; }
        }

        // The PostedSQL class represents the data structure of the POST request body
        public class PostedSQL
        {
            // The identifier of the predefined SQL statement to execute
            public string IdentQBMLimitedSQL { get; set; }
            // Additional parameters to pass to the SQL statement
            public string uid_department { get; set; }
            public string uid_head { get; set; }

            public string userInserted { get; set; }

        }
    }
}
