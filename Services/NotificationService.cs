using Blazored.Toast.Services;
using DiceGame.Model;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Services
{
    public class NotificationService
    {
        public bool NotifyOnDice { get; set; } = false;
        public bool NotifyOnMove { get; set; } = true;
        public bool NotifyOnPossibleKick { get; set; } = true;
        public bool NotifyOnKick { get; set; } = true;

        public void OnDice(IToastService toastService, String dice, String playerName)
        {
            if (NotifyOnDice)
            {
                //toastService.ShowInfo(dice + " was diced!", playerName);
                RenderFragment message = builder =>
                {
                    builder.OpenElement(0, "text");
                    builder.OpenElement(1, "span");
                    builder.AddAttribute(2, "style", "font-family: Symbola, Code2000, DejaVu Sans, Droid Sans Fallback, EversonMono, STIXGeneral, Quivira, Unicode BMP Fallback SIL; font-size: 32px;");
                    builder.AddContent(3, dice);
                    builder.CloseElement();
                    builder.AddContent(4, " was diced!");
                    builder.CloseElement();                    
                };



                //"<text><span style=\"font-family: Symbola, Code2000, DejaVu Sans, Droid Sans Fallback, EversonMono, STIXGeneral, Quivira, Unicode BMP Fallback SIL; font-size: 28px; \">" + dice + "</span> was diced</text>"
                toastService.ShowToast(ToastLevel.Info, message, playerName);
            }
        }

        public void OnMove(IToastService toastService, MoveModel move, String playerName)
        {
            if (NotifyOnMove)
            {
                toastService.ShowInfo("Moved!", playerName);
                if (move.ThrownMeeple != null)
                {
                    OnKick(toastService, move, playerName);
                }
            }
        }

        public void OnKick(IToastService toastService, MoveModel move, String playerName)
        {
            if (NotifyOnKick)
            {
                toastService.ShowWarning("kicked '" + move.ThrownMeeple.Player.Name + "'.", playerName);
            }
        }

        public void OnPossibleKick(IToastService toastService, IEnumerable<MoveModel> possibleMoves, String playerName)
        {
            if (NotifyOnPossibleKick && possibleMoves.Any())
            {
                foreach (var move in possibleMoves)
                {
                    toastService.ShowWarning("May kick '" + move.ThrownMeeple.Player.Name + "'.", playerName);
                }
            }
        }
    }
}
