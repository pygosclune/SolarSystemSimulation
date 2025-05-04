using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SolarSystemSimulation;

public partial class MainWindow : Window
{
    private DispatcherTimer animationTimer;
    private List<CelestialBody> celestialBodies;
    private Point canvasCenter;
    private double simulationTimeScale = 1.0;
    private double distanceScale = 1.0;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        SetupTimer();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        InitializeSimulation();
        animationTimer.Start();
    }


    private void InitializeSimulation()
    {
        SolarSystemCanvas.Children.Clear();
        celestialBodies = new List<CelestialBody>();

        var sun = new CelestialBody("Sun", Colors.Yellow, 20);
        celestialBodies.Add(sun);

        celestialBodies.Add(new CelestialBody("Mercury", Colors.Gray, 4, 50, 88));
        celestialBodies.Add(new CelestialBody("Venus", Colors.Orange, 8, 90, 225));
        celestialBodies.Add(new CelestialBody("Earth", Colors.Blue, 9, 130, 365));
        celestialBodies.Add(new CelestialBody("Mars", Colors.Red, 6, 180, 687));

        simulationTimeScale = 30.0;

        foreach (var body in celestialBodies)
        {
            if (body.OrbitShape != null)
            {
                SolarSystemCanvas.Children.Add(body.OrbitShape);
            }
            SolarSystemCanvas.Children.Add(body.Element);
        }
        
        if (SolarSystemCanvas.ActualWidth > 0 && SolarSystemCanvas.ActualHeight > 0)
        {
            UpdateLayoutBasedOnSize(new Size(SolarSystemCanvas.ActualWidth, SolarSystemCanvas.ActualHeight));
        }
    }

    private void SetupTimer()
    {
        animationTimer = new DispatcherTimer();
        animationTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0); //60 FPS
        animationTimer.Tick += AnimationTimer_Tick;
    }

    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
        if (celestialBodies == null || canvasCenter == default) return;

        double deltaTimeInDays = animationTimer.Interval.TotalSeconds * simulationTimeScale;

        foreach (var body in celestialBodies)
        {
            if (body.IsCenter)
            {
                PositionElement(body.Element, canvasCenter.X, canvasCenter.Y, body.VisualRadius);
                continue;
            }

            double angularVelocity = (2 * Math.PI) / body.OrbitalPeriod;
            body.CurrentAngle += angularVelocity * deltaTimeInDays;
            body.CurrentAngle %= (2 * Math.PI);

            double scaledOrbitalRadius = body.OrbitalRadius * distanceScale;
            double relativeX = scaledOrbitalRadius * Math.Cos(body.CurrentAngle);
            double relativeY = scaledOrbitalRadius * Math.Sin(body.CurrentAngle);

            double canvasX = canvasCenter.X + relativeX;
            double canvasY = canvasCenter.Y + relativeY;

            PositionElement(body.Element, canvasX, canvasY, body.VisualRadius);
        }
    }

    private void PositionElement(FrameworkElement element, double centerX, double centerY, double radius)
    {
        Canvas.SetLeft(element, centerX - radius);
        Canvas.SetTop(element, centerY - radius);
    }

    private void PositionOrbit(Ellipse orbitShape, double centerX, double centerY, double scaledRadius)
    {
        orbitShape.Width = scaledRadius * 2;
        orbitShape.Height = scaledRadius * 2;
        Canvas.SetLeft(orbitShape, centerX - scaledRadius);
        Canvas.SetTop(orbitShape, centerY - scaledRadius);
    }

    private void SolarSystemCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateLayoutBasedOnSize(e.NewSize);
    }

    private void UpdateLayoutBasedOnSize(Size newSize)
    {
        if (newSize.Width <= 0 || newSize.Height <= 0) return;
        canvasCenter = new Point(newSize.Width / 2, newSize.Height / 2);
        if (celestialBodies == null || celestialBodies.Count == 0) return;

        double maxOrbitalRadius = 0;
        CelestialBody farthestBody = null;
        foreach (var body in celestialBodies)
        {
            if (!body.IsCenter && body.OrbitalRadius > maxOrbitalRadius)
            {
                maxOrbitalRadius = body.OrbitalRadius;
                farthestBody = body;
            }
        }

        double halfMinDimension = Math.Min(newSize.Width, newSize.Height) / 2.0;
        if (maxOrbitalRadius > 0 && farthestBody != null)
        {
            double margin = farthestBody.VisualRadius + 10;
            double availableRadius = halfMinDimension - margin;

            if (availableRadius > 0)
            {
                distanceScale = availableRadius / maxOrbitalRadius;
            }
            else
            {
                distanceScale = 0.01;
            }
        }
        else
        {
            distanceScale = 1.0;
        }

        UpdateAllPositionsAndOrbits();
    }
    
    private void UpdateAllPositionsAndOrbits()
    {
         if (celestialBodies == null || canvasCenter == default) return;

         foreach (var body in celestialBodies)
         {
             if (body.IsCenter)
             {
                 PositionElement(body.Element, canvasCenter.X, canvasCenter.Y, body.VisualRadius);
             }
             else
             {
                 double scaledOrbitalRadius = body.OrbitalRadius * distanceScale;

                 if (body.OrbitShape != null)
                 {
                     PositionOrbit(body.OrbitShape, canvasCenter.X, canvasCenter.Y, scaledOrbitalRadius);
                 }

                 double relativeX = scaledOrbitalRadius * Math.Cos(body.CurrentAngle);
                 double relativeY = scaledOrbitalRadius * Math.Sin(body.CurrentAngle);
                 double canvasX = canvasCenter.X + relativeX;
                 double canvasY = canvasCenter.Y + relativeY;

                 PositionElement(body.Element, canvasX, canvasY, body.VisualRadius);
             }
         }
    }
}