﻿using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamServices.Samples.Client.Build
{
    [ClientSample(BuildResourceIds.AreaName, BuildResourceIds.BuildsResource)]
    public class BuildsSample : ClientSample
    {
        [ClientSampleMethod]
        public async Task<BuildDefinition> CloneABuildAsync(int definitionToCloneId,string projectName)
        {
            // Get a build client instance
            VssConnection connection = Context.Connection;
            BuildHttpClient buildClient = connection.GetClient<BuildHttpClient>();

            var buildToClone = await buildClient.GetDefinitionAsync(
                  projectName,
                  definitionToCloneId);

            var x= await buildClient.CreateDefinitionAsync(new BuildDefinition { Name = "Fred", Process= buildToClone.Process, Project= buildToClone.Project,Repository= buildToClone.Repository }, definitionToCloneId);
            return x;
        }

        [ClientSampleMethod]

        public IEnumerable<BuildDefinitionReference> ListBuildDefinitions()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a build client instance
            VssConnection connection = Context.Connection;
            BuildHttpClient buildClient = connection.GetClient<BuildHttpClient>();

            List<BuildDefinitionReference> buildDefinitions = new List<BuildDefinitionReference>();

            // Iterate (as needed) to get the full set of build definitions
            string continuationToken = null;
            do
            {
                IPagedList<BuildDefinitionReference> buildDefinitionsPage = buildClient.GetDefinitionsAsync2(
                    project: projectName,
                    continuationToken: continuationToken).Result;

                buildDefinitions.AddRange(buildDefinitionsPage);

                continuationToken = buildDefinitionsPage.ContinuationToken;
            } while (!String.IsNullOrEmpty(continuationToken));

            // Show the build definitions
            foreach (BuildDefinitionReference definition in buildDefinitions)
            {
                Console.WriteLine("{0} {1}", definition.Id.ToString().PadLeft(6), definition.Name);
            }

            return buildDefinitions;
        }
    }
}
