using FusionDemo.HealthCentral.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Authentication;
using Stl.Fusion.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Host.Controllers
{
    [Route("api/painel-composer")]
    [ApiController, JsonifyErrors]
    public class PainelComposerController : ControllerBase, IPainelComposerService
    {

        private readonly IPainelComposerService _composer;

        public PainelComposerController(IPainelComposerService composer) => _composer = composer;


        [HttpGet("get"), Publish]
        public Task<PainelComposedValue> GetComposedValueAsync(string parameter,  CancellationToken cancellationToken = default)
        {
            parameter ??= "";
            return _composer.GetComposedValueAsync(parameter, cancellationToken);
        }

    }
}
