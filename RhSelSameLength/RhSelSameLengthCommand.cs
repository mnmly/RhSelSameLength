using System;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using Eto.Drawing;
using Eto.Forms;
using Rhino.DocObjects;
using System.Linq;

namespace mnml
{
    public class RhSelSameLengthCommand : Rhino.Commands.Command
    {
        public override string EnglishName => "SelSameLength";
            
        protected override Result RunCommand(Rhino.RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will select the curve with same length", EnglishName);
            double tolerance = 0.001;
                Rhino.Commands.Result rc = Rhino.Commands.Result.Failure;

            using (GetObject getObjectAction = new GetObject())
            {
                getObjectAction.GeometryFilter = ObjectType.Curve;
                getObjectAction.SetCommandPrompt("Please select curves");
                var dblOption = new OptionDouble(tolerance, true, 0);
                getObjectAction.AddOptionDouble("SelectionTolerance", ref dblOption);
                getObjectAction.EnablePreSelect(true, true);
                var result = getObjectAction.Get();
                double refLength = 0.0;
                if (result == GetResult.Object)
                {
                    var value = getObjectAction.Object(0);
                    var crv = value.Curve();
                    if (crv != null)
                    {
                        refLength = crv.GetLength();
                        var objects = doc.Objects.FindByObjectType(ObjectType.Curve);
                        foreach(var obj in objects)
                        {
                            var _curve = (new ObjRef(obj)).Curve();
                            if (Rhino.RhinoMath.EpsilonEquals(_curve.GetLength(), refLength, dblOption.CurrentValue))
                            {
                                obj.Select(true);
                            }
                        }
                        rc = Rhino.Commands.Result.Success;
                    }
                }
            }
            doc.Views.Redraw();
            return rc;
   
        }
    }
}