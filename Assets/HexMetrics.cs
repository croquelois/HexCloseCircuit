using UnityEngine;

public class HexMetrics {
    public const float outerRadius = 10f;
    public const float innerRadius = outerRadius * 0.866025404f;
    public const float depth = 2f;
    public const float carveWidth = 0.2f;
    public const float carveDepth = 1f;
    public const float solidFactor = 0.9f;
    
    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };
    public static Vector3[] neighbor = {
        new Vector3(innerRadius, 0f, 1.5f*outerRadius), // NE 
        new Vector3(innerRadius*2f, 0f, 0f), // E
        new Vector3(innerRadius, 0f, -1.5f*outerRadius), // SE
        new Vector3(-innerRadius, 0f, -1.5f*outerRadius), // SW
        new Vector3(-innerRadius*2f, 0f, 0f), // W
        new Vector3(-innerRadius, 0f, 1.5f*outerRadius), // NW
    };
        
    public static Vector3 GetNeighbor(HexDirection direction) {
        return neighbor[(int)direction];
    }
    
    public static Vector3 GetFirstCorner(HexDirection direction) {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction) {
        return corners[(int)direction + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection direction) {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction) {
        return corners[(int)direction + 1] * solidFactor;
    }
    
    public static Vector3 GetBridge(HexDirection direction) {
        return (corners[(int)direction] + corners[(int)direction + 1]) * (1f - solidFactor);
    }
}
