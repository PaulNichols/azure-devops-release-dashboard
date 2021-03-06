﻿/*
 * Releases API
 *
 * * View the Release status for a Dev Ops Team Project * Compare Environments * Trigger Releases of multiple Porjects to an Environment
 *
 * OpenAPI spec version: 1.0.0
 * Contact: pauljamesnichols@gmail.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using IO.Swagger.Attributes;
using IO.Swagger.Models;
using Swashbuckle.Swagger.Annotations;
using Microsoft.TeamServices.Samples.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamServices.Samples.Client.Release;
using Microsoft.VisualStudio.Services.Common;
using System.Net;
using System.Threading.Tasks;
using static IO.Swagger.Models.ReleaseEnvironmentInfo;
using Microsoft.TeamServices.Samples.Client.Git;
using Microsoft.TeamServices.Samples.Client.Build;
using Microsoft.TeamServices.Samples.Client.WorkItemTracking;
using System.Linq;

namespace Api
{
    /// <summary>
    /// 
    /// </summary>
    public class ReleaseController : Controller
    {
        /// <summary>
        /// Takes from and to env then returns list of differences.
        /// </summary>

        /// <param name="environmentFrom"></param>
        /// <param name="environmentTo"></param>
        /// <response code="200">A list of comparision results</response>
        [HttpGet]
        [Route("/api/releases/compare/{environmentFrom}/{environmentTo}")]
        [ValidateModelState]
        [SwaggerOperation("Compare")]
        [SwaggerResponse(statusCode: 200, type: typeof(ComparisionResults), description: "A list of comparision results")]
        public async virtual Task<IActionResult> Compare([FromRoute][Required]string environmentFrom, [FromRoute][Required]string environmentTo)
        {
            var sample = new ReleasesSample
            {
                Context = ClientSampleContext.CurrentContext
            };

            var releases = await sample.ListAllReleaseDefinitionsWithEnvironmentsExpanded($"{environmentFrom},{environmentTo}", null);
            var returnReleases = new List<ReleaseInfo> { };

            var Exclusions = new [] { "Release Tool", "Clean previous deployments" };

            //additional filtering for temporary
            releases = releases.Where(x => !x.PipelineName.ToLower().Contains("tests")
                        && !x.PipelineName.ToUpper().EndsWith("POC")
                        && !Exclusions.Any(ex => ex.Equals(x.PipelineName, StringComparison.OrdinalIgnoreCase)))
                        .ToList();

            foreach (var release in releases)
            {
                var temp = release.Environments.ConvertAll(x => new ReleaseEnvironmentInfo
                {
                    DefinitionEnvironmentId = x.DefinitionEnvironmentId,
                    Name = x.Name,
                    Status = x.Status == null ? StatusEnum.NotStarted : (StatusEnum)Enum.Parse(typeof(StatusEnum), x.Status),
                    Version = x.Version,
                    When = x.When,
                    Who = x.Who,
                    Rank = x.Rank,
                    EnvironmentId = x.Id,
                    ReleaseId = x.ReleaseId,
                });

                var fromVersion = new Version(temp.First().Version);
                var toVersion = new Version(temp.Last().Version);
                var difference = fromVersion.CompareTo(toVersion);

                if (difference > 0)
                    temp.Last().Status = StatusEnum.Scheduled;
                else if (difference < 0)
                    temp.First().Status = StatusEnum.Scheduled;

                returnReleases.Add(new ReleaseInfo
                {
                    ProjectName = release.PipelineName,
                    Environments = temp
                });
            }

            return new ObjectResult(returnReleases);
        }

        /// <summary>
        /// Deploys all or specified projects from one environment to another
        /// </summary>

        /// <param name="environmentFrom"></param>
        /// <param name="environmentTo"></param>
        /// <response code="200">Blah</response>
        [HttpPost]
        [Route("/api/releases/deploy")]
        [ValidateModelState]
        [SwaggerOperation("DeployPost")]
        public async virtual Task<IActionResult> DeployPost([FromRoute][Required]string environmentFrom, [FromRoute][Required]string environmentTo)
        {
            var sample = new ReleasesSample();
            sample.Context = ClientSampleContext.CurrentContext;

            //  await sample.StartDeployment(2,"",1);

            return null;
        }
        public async virtual Task<IActionResult> CloneBuild(int definitionToCloneId, string projectName)
        {
            var sample = new BuildsSample();
            sample.Context = ClientSampleContext.CurrentContext;

            return new ObjectResult(await sample.CloneABuildAsync(definitionToCloneId, projectName));
        }

        public async virtual Task<IActionResult> CreateRepo(string newRepoName, string nameOfProjectToCopyFrom, string nameOfRepoToCopyFrom)
        {
            var sample = new RepositoriesSample();
            sample.Context = ClientSampleContext.CurrentContext;

            return new ObjectResult(await sample.CreateRepo(newRepoName, nameOfProjectToCopyFrom, nameOfRepoToCopyFrom));
        }

        public async virtual Task<IActionResult> PerformMassRelease(string projectName, int releaseId, int definitionEnvironmentId)
        {
            var sample = new ReleasesSample();
            sample.Context = ClientSampleContext.CurrentContext;

            return new ObjectResult(await sample.DeployReleaseToEnvironment(projectName, releaseId, definitionEnvironmentId));
        }

        /// <summary>
        /// Generates notes for all or specified projects from one env to another
        /// </summary>

        /// <response code="200">A markdown string</response>
        [HttpGet]
        [Route("/api/releases/notes")]
        [ValidateModelState]
        [SwaggerOperation("ReleaseNotes")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "A markdown string")]
        public virtual IActionResult ReleaseNotes()
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(string));

            string exampleJson = null;
            exampleJson = "\"\"";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<string>(exampleJson)
            : default(string);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Returns current version number, status, date and who deployed for all projects in all or just specified environment.
        /// </summary>

        /// <response code="200">A list of Release details</response>
        [HttpGet]
        [Route("/api/releases/")]
        [ValidateModelState]
        [SwaggerOperation("RootGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ReleaseInfo>), description: "A list of Release details")]
        public async virtual Task<IActionResult> Get(string environments = null, int? definitionId = null)
        {
            var sample = new ReleasesSample
            {
                Context = ClientSampleContext.CurrentContext
            };

            List<ReleasesSample.ReleaseInfo> releases = await sample.ListAllReleaseDefinitionsWithEnvironmentsExpanded(environments, definitionId);
            List<ReleaseInfo> returnReleases = new List<ReleaseInfo> { };

            foreach (var release in releases)
            {
                returnReleases.Add(new ReleaseInfo
                {
                    ProjectName = release.PipelineName,

                    Environments = release.Environments.ConvertAll(x =>
                     new ReleaseEnvironmentInfo
                     {
                         DefinitionEnvironmentId = x.DefinitionEnvironmentId,
                         Name = x.Name,
                         Status = x.Status == null ? StatusEnum.Undefined : (StatusEnum)Enum.Parse(typeof(StatusEnum), x.Status),
                         Version = x.Version,
                         When = x.When,
                         Who = x.Who,
                         Rank = x.Rank,
                         EnvironmentId = x.Id,
                         ReleaseId = x.ReleaseId,
                     }),
                });
            }

            return new ObjectResult(returnReleases);
        }

        public async virtual Task<IActionResult> CreateStoryDoneAPI(string project)
        {
            var sample = new WorkItemsSample();
            sample.Context = ClientSampleContext.CurrentContext;
            await sample.CreateStoryDoneAPI(project, "EAP-001 - Done - Test", "3");
            return new ObjectResult(true);
        }
    }
}