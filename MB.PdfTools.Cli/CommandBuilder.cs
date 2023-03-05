using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.PdfTools.Cli
{
    public class CommandBuilder
    {
        public ICommand BuildCommand(CommandContext ctx)
        {
            ArgumentNullException.ThrowIfNull(ctx);

            switch (ctx.Command)
            {
                case Command.Merge:
                    if (ctx.Options.OutFile == null) throw new ArgumentNullException("OutFile");
                    return new MergeCommand(new MergeCommandParameters(ctx.Files, ctx.Options.OutFile, ctx.Options.Orientation));
                case Command.Spit:
                    if (ctx.Options.OutFile == null) throw new ArgumentNullException("OutFile");
                    return new SplitCommand(new SplitCommandParameters(ctx.Files, ctx.Options.OutFile));
                default:
                    throw new Exception("Unknown command.");
            }
        }
    }
}
