using System.Collections.Generic;
using UnityEngine;

namespace AE_FSM
{
    public enum Styles
    {
        Normal = 0,
        Bule,
        Miut,
        Green,
        Yellow,
        Orange,
        Red,
        NormalOn,
        BuleOn,
        MiutOn,
        GreenOn,
        YellowOn,
        OrangeOn,
        RedOn,
    }

    public class StateStyle
    {
        private Dictionary<Styles, GUIStyle> styles;

        public StateStyle()
        {
            styles = new Dictionary<Styles, GUIStyle>();
            for (int i = 0; i <= 6; i++)
            {
                styles.Add((Styles)i, new GUIStyle($"flow node {i}"));
                styles.Add((Styles)(i + 7), new GUIStyle($"flow node {i} on"));
            }
        }

        public GUIStyle GetStyle(Styles style)
        {
            return styles[style];
        }

        public void ApplyZoomFactory(float zoomFactory)
        {
            foreach (GUIStyle item in styles.Values)
            {
                item.fontSize = (int)Mathf.Lerp(5, 30, zoomFactory);
                item.contentOffset = new Vector2(0, Mathf.Lerp(-30, -20, zoomFactory));
            }
        }
    }
}