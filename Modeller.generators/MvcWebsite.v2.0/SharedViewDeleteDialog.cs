using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewDeleteDialog : IGenerator
    {
        private readonly Module _module;

        public SharedViewDeleteDialog(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("@model DeleteDialogViewModel");
            sb.AppendLine();
            sb.AppendLine("<div class=\"modal fade\" id=\"divDeleteDialog\" tabindex=\"-1\" role=\"dialog\" data-backdrop=\"static\" data-keyboard=\"false\">");
            sb.AppendLine("    <div class=\"modal-dialog\" role=\"document\">");
            sb.AppendLine("        <div class=\"modal-content\">");
            sb.AppendLine("            <div class=\"modal-header\">");
            sb.AppendLine("                <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>");
            sb.AppendLine("                <h4 class=\"modal-title\" id=\"myModalLabel\">Delete Confirmation</h4>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"modal-body\">");
            sb.AppendLine("                <div class=\"row\">");
            sb.AppendLine("                    <div class=\"pull-left\" style=\"padding: 0 20px 0 20px;\">");
            sb.AppendLine("                        <i class=\"far fa-trash-alt fa-2x\"></i>");
            sb.AppendLine("                    </div>");
            sb.AppendLine("                    <div class=\"pull-left\" style=\"font-size:20px;\">");
            sb.AppendLine("                        Are you sure you want to delete this record?");
            sb.AppendLine("                    </div>");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            @{");
            sb.AppendLine("                if (Model.IsGridDelete)");
            sb.AppendLine("                {");
            sb.AppendLine("                    <div class=\"modal-footer\">");
            sb.AppendLine("                        <a class=\"btn btn-outline btn-danger\" href=\"javascript: gridrow_confirmed();\" id=\"detailForm-BtnGridDelete\"><i class=\"far fa-trash-alt\"></i> Delete</a>");
            sb.AppendLine("                        <a class=\"btn btn-outline btn-default\" data-dismiss=\"modal\"><i class=\"far fa-times-circle\"></i> Cancel</a>");
            sb.AppendLine("                    </div>");
            sb.AppendLine("                }");
            sb.AppendLine("                else");
            sb.AppendLine("                {");
            sb.AppendLine("                    <form asp-action=\"@Model.Action\">");
            sb.AppendLine("                        <input type=\"hidden\" value=\"@Model.BusinessKey\" name=\"Id\" />");
            sb.AppendLine();
            sb.AppendLine("                        <div class=\"modal-footer\">");
            sb.AppendLine("                            <button type=\"submit\" class=\"btn btn-outline btn-danger\"><i class=\"far fa-trash-alt\"></i> Delete</button>");
            sb.AppendLine("                            <a class=\"btn btn-outline btn-default\" data-dismiss=\"modal\"><i class=\"far fa-times-circle\"></i> Cancel</a>");
            sb.AppendLine("                        </div>");
            sb.AppendLine("                    </form>");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return new File
            {
                Name = "_DeleteDialog.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
