using System.Windows.Media;
using System.Windows.Shapes;

namespace SolarSystemSimulation;

public class CelestialBody
{
    public string Name { get; set; }
    public Ellipse Element { get; set; }
    public Ellipse OrbitShape { get; set; }

    public Color BodyColor { get; set; }
    public double VisualRadius { get; set; }
    public double OrbitalRadius { get; set; }
    public double OrbitalPeriod { get; set; }

    public double CurrentAngle { get; set; }
    public bool IsCenter { get; set; } = false;

    public CelestialBody(string name, Color color, double visualRadius, double orbitalRadius = 0, double orbitalPeriod = 1, double startAngleDeg = 0)
    {
        Name = name;
        BodyColor = color;
        VisualRadius = visualRadius;
        OrbitalRadius = orbitalRadius;
        OrbitalPeriod = orbitalPeriod > 0 ? orbitalPeriod : 1;
        CurrentAngle = startAngleDeg * (Math.PI / 180.0);
        IsCenter = orbitalRadius == 0;

        Element = new Ellipse
        {
            Width = visualRadius * 2,
            Height = visualRadius * 2,
            Fill = new SolidColorBrush(color),
            ToolTip = name
        };

        if (!IsCenter)
        {
            OrbitShape = new Ellipse
            {
                Stroke = Brushes.DarkGray,
                StrokeThickness = 1
            };
        }
        else
        {
            OrbitShape = null;
        }
    }
}