﻿using MudBlazor;

namespace ClientApp.Managers.Preferences
{
    public static class BlazorHeroTheme
    {
        private static readonly Typography DefaultTypography = new()
        {
            Default = new Default()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            H1 = new H1()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "6rem",
                FontWeight = 300,
                LineHeight = 1.167,
                LetterSpacing = "-.01562em"
            },
            H2 = new H2()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "3.75rem",
                FontWeight = 300,
                LineHeight = 1.2,
                LetterSpacing = "-.00833em"
            },
            H3 = new H3()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "3rem",
                FontWeight = 400,
                LineHeight = 1.167,
                LetterSpacing = "0"
            },
            H4 = new H4()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "2.125rem",
                FontWeight = 400,
                LineHeight = 1.235,
                LetterSpacing = ".00735em",
                
            },
            H5 = new H5()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.5rem",
                FontWeight = 500,
                LineHeight = 1.334,
                LetterSpacing = "0"
            },
            H6 = new H6()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1rem",
                FontWeight = 600,
                LineHeight = 1.3,
                LetterSpacing = ".0075em"
            },
            Button = new Button()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 500,
                LineHeight = 1.75,
                LetterSpacing = ".02857em"
            },
            Body1 = new Body1()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1rem",
                FontWeight = 400,
                LineHeight = 1.5,
                LetterSpacing = ".00938em"
            },
            Body2 = new Body2()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            Caption = new Caption()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".75rem",
                FontWeight = 400,
                LineHeight = 1.66,
                LetterSpacing = ".03333em",
            },
            Subtitle2 = new Subtitle2()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 500,
                LineHeight = 1.57,
                LetterSpacing = ".00714em"
            }
        };

        private static LayoutProperties DefaultLayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "3px"
        };

        public static readonly MudTheme DefaultTheme = new MudTheme()
        {
            Palette = new PaletteLight()
            {
                Primary = "#8315F9",
                PrimaryLighten = "#9535fc",
                Secondary = "#8BF915",
                SecondaryLighten = "#A1FF00",
                Error = "#E62019",
                AppbarBackground = "#8AAAE5",
                SecondaryDarken = "#EA738D",
                Warning = "#FBBC05",
                Black = "#1A1A1A",
                TextPrimary = "#1A1A1A",
                Background = Colors.Grey.Lighten5,
                DrawerBackground = "#FFF",
                DrawerText = "rgba(0,0,0, 0.7)",
                Success = "#04e762",
            },
            Typography = DefaultTypography,
            LayoutProperties = DefaultLayoutProperties
        };

        public static MudTheme DarkTheme = new MudTheme()
        {
            Palette = new Palette()
            {
                Primary = "#38b000",
                PrimaryLighten = "#e1f0d4",
                Error = "#a53939",
                Success = "#04e762",
                Black = "#27272f",
                Background = "#32333d",
                BackgroundGrey = "#27272f",
                Surface = "#373740",
                DrawerBackground = "#27272f",
                DrawerText = "rgba(255,255,255, 0.50)",
                AppbarBackground = "#373740",
                AppbarText = "rgba(255,255,255, 0.70)",
                TextPrimary = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                DrawerIcon = "rgba(255,255,255, 0.50)"
            },
            Typography = DefaultTypography,
            LayoutProperties = DefaultLayoutProperties
        };
    }
}