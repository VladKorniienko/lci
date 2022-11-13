using LCI_Korniienko.Data;
using Microsoft.AspNetCore.Mvc;
using LCI_Korniienko.Evaluation;

namespace LCI_Korniienko.Controllers
{
    [Route("api/interpreter")]
    [ApiController]
    public class InterpreterController : ControllerBase
    {
        // POST api/interpreter
        [HttpPost]
        public IActionResult Post([FromBody] CommandDto inputString)
        {
            var newCom = new CommandDto()
            {
                CommandString = inputString.CommandString
            };

            string result = InputProcessing.Run(newCom);
            
            return Ok(new
            {
                Result = result
            });
        }

    }
}
