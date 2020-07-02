using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TheDiceApi.Managers;
using TheDiceApi.Models;
using TheDiceApi.ViewModels;

namespace TheDiceApi.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiceRollerController : ControllerBase
    {
        private readonly IDiceRollManager _manager;

        private const string RollDice = nameof(RollDice);
        private const string RollDiceFriendlyName = "Roll Dice";


        public DiceRollerController(IDiceRollManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Gets the available commands for a dice roll of a single specific dice type
        /// </summary>
        /// <returns>DiceRollInfoComponent</returns>
        /// <response code="200">Returns Available Commands</response>
        [HttpGet("singleDiceRollCommandsInfo")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<DiceRollInfoComponent>> GetSingleDiceRollCommands()
        {
            var diceRollComponents = new List<DiceRollInfoComponent>();

            foreach (var diceType in Enum.GetNames(typeof(DieType)))
            {
                diceRollComponents.Add(new DiceRollInfoComponent
                {
                    AvailableCommands = new List<CommandComponent>{ GetNewSingleRollComponent(diceType) }
                });
            }
            return diceRollComponents;
        }

        /// <summary>
        ///  Gets the available commands for a dice rolls of all dice types
        /// </summary>
        /// <returns>DiceRollInfoComponent</returns>
        /// <response code="200">Returns Available Commands</response>
        [HttpGet("multipleDiceRollCommandsInfo")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<DiceRollInfoComponent> GetMultipleDiceRollCommands()
        {
            var diceRollComponent = new DiceRollInfoComponent
            {
                AvailableCommands = new List<CommandComponent> { GetNewMultipleRollsComponent() }
            };


            return diceRollComponent;
        }

        /// <summary>
        /// Gets dice rolls for a given dice type and count
        /// </summary>
        /// <param name="dieType"></param>
        /// <param name="count"></param>
        /// <returns>List&lt;Die&gt;</returns>
        /// <response code="200">Returns a list of dice rolls</response>
        /// <response code="400">When Count is 0</response>
        [HttpGet("die/{dieType:regex(d4|d8|d6|d12|d20|d100|d10)}/rollSingle/count={count:long}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Die>> GetSingle(DieType dieType, uint count)
        {
            if (count == 0) return BadRequest("Invalid Count");

            var result = _manager.RollDie(dieType, count);
            return result;
        }

        /// <summary>
        /// Gets dice rolls for a given dice type utilizing a selected command
        /// </summary>
        /// <param name="component">A command component with information necessary to process the request</param>
        /// <returns>SingleDiceRollComponent</returns>
        /// <response code="200">Returns a component with the requested rolls and available commands</response>
        /// <response code="400">When SelectedCommand or Payload are null, count is 0, or invalid dice type provided</response>
        [HttpPost("die/rollSingleWithCommand")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<SingleDiceRollComponent> PostSingleWithCommand([FromBody] SingleDiceRollComponent component)
        {
            if (component.SelectedCommand == null)
            {
                return BadRequest("No selected command provided.");
            }

            var request = component.SelectedCommand.Payload.ExecutionData.ToObject<DiceRollRequest>();

            if (request == null || request.RollCount == 0 || !Enum.TryParse(typeof(DieType), request.DieType, out var dieType))
            {
                return BadRequest("Invalid payload");
            }

            var rolls = _manager.RollDie((DieType)dieType, request.RollCount);

            component.RollValues = rolls.Select(x => x.RollValue).ToList();
            component.AvailableCommands.Clear();
            component.AvailableCommands.Add(GetNewSingleRollComponent(component.DieType));
            return component;
        }

        /// <summary>
        /// Gets dice rolls for a given dice types and counts
        /// </summary>
        /// <param name="d4Count"></param>
        /// <param name="d6Count"></param>
        /// <param name="d8Count"></param>
        /// <param name="d10Count"></param>
        /// <param name="d12Count"></param>
        /// <param name="d20Count"></param>
        /// <param name="d100Count"></param>
        /// <returns>List&lt;DiceRolls&gt;</returns>
        /// <response code="200">Returns a list of dice rolls</response>
        [HttpGet("die/rollMultiple/d4Count={d4Count:long}&d6Count={d6Count:long}&d8Count={d8Count:long}&d10Count={d10Count:long}&d12Count={d12Count:long}&d20Count={d20Count:long}&d100Count={d100Count:long}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<DiceRolls>> GetMultiple(
            uint d4Count, 
            uint d6Count, 
            uint d8Count, 
            uint d10Count, 
            uint d12Count, 
            uint d20Count, 
            uint d100Count)
        {
            var diceRolls = new List<DiceRolls>();

            if (d4Count != 0)
            {
                AddRolls(DieType.D4, d4Count, diceRolls);
            }

            if (d6Count != 0)
            {
                AddRolls(DieType.D6, d6Count, diceRolls);
            }

            if (d8Count != 0)
            {
                AddRolls(DieType.D8, d8Count, diceRolls);
            }

            if (d10Count != 0)
            {
                AddRolls(DieType.D10, d10Count, diceRolls);
            }

            if (d12Count != 0)
            {
                AddRolls(DieType.D12, d12Count, diceRolls);
            }

            if (d20Count != 0)
            {
                AddRolls(DieType.D20, d20Count, diceRolls);
            }

            if (d100Count != 0)
            {
                AddRolls(DieType.D100, d100Count, diceRolls);
            }


            return diceRolls;
        }

        /// <summary>
        /// Gets dice rolls for a given dice types utilizing commands
        /// </summary>
        /// <param name="component">A command component with information necessary to process the request</param>
        /// <returns></returns>
        /// <response code="200">Returns a component with the requested rolls and available commands</response>
        /// <response code="400">When SelectedCommand or Payload are null, count is 0, or invalid dice type provided</response>
        [HttpPost("die/rollManyWithCommand")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<MultipleDiceRollComponent> PostManyWithCommand([FromBody] MultipleDiceRollComponent component)
        {
            if (component.SelectedCommand == null)
            {
                return BadRequest("No selected command provided.");
            }

            var request = component.SelectedCommand.Payload.ExecutionData.ToObject<Dictionary<string, uint>>();

            if (request == null || !request.Any())
            {
                return BadRequest("Invalid payload");
            }

            var rolls = _manager.RollDice(request);

            component.RollValues = rolls;
            component.AvailableCommands.Clear();
            component.AvailableCommands.Add(GetNewMultipleRollsComponent());
            return component;
        }

        private void AddRolls(DieType dieType, uint count, List<DiceRolls> diceRolls)
        {
            var rollRequest = new Dictionary<string, uint> {{ dieType.ToString(), count}};
            var result = _manager.RollDice(rollRequest);
            diceRolls.Add(result);
        }

        private CommandComponent GetNewSingleRollComponent(string diceType)
        {
            return new CommandComponent
            {
                CommandName = RollDice,
                FriendlyName = RollDiceFriendlyName,
                IsEnabled = true,
                IsVisible = true,
                Payload = new CommandPayload
                {
                    ExecutionData = JObject.FromObject(new DiceRollRequest { DieType = diceType })
                }
            };
        }

        private CommandComponent GetNewMultipleRollsComponent()
        {
            var options = new Dictionary<string, uint>();

            foreach (var die in Enum.GetValues(typeof(DieType)))
            {
                options.Add(((DieType)die).ToString(), 0);
            }

            return new CommandComponent
            {
                CommandName = RollDice,
                FriendlyName = RollDiceFriendlyName,
                IsEnabled = true,
                IsVisible = true,
                Payload = new CommandPayload
                {
                    ExecutionData = JObject.FromObject(options)
                }
            };
        }
    }
}
