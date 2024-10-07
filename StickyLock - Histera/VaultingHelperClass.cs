// Author: Sjors Thomassen, StickyLock

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public static class VaultingHelperClass
{
    #region Vault Conditions
    /// <summary>
    /// Find a valid wall for vaulting within a specified range.
    /// </summary>
    /// <param name="collisionWorld">The collision world for raycasting</param>
    /// <param name="collisionFilter">The filter used for the raycast</param>
    /// <param name="position">Players position</param>
    /// <param name="forward">Players forward direction</param>
    /// <param name="vaultSettings">settings needed to define a valid vaultable object</param>
    /// <param name="vaultState">state to set values during vault</param>
    /// <param name="wallPosition">the detected position of the vaultable object</param>
    /// <returns>True when a valid wall is found</returns>
    public static bool ValidateVaultLedge(in CollisionWorld collisionWorld, in CollisionFilter collisionFilter,
        in float3 position, in float3 forward, in AbilityVaultSettings vaultSettings,
        ref AbilityVaultState vaultState, out float3 wallPosition)
    {
        if (GetVaultPosition(collisionWorld, collisionFilter, position, forward, vaultSettings, out wallPosition))
        {
            if (GetVaultHeight(collisionWorld, collisionFilter, wallPosition, position, forward, vaultSettings, out var ledgePosition)) // move the ledge position a little bit from the player away, to get the position more within the ledge
            {
                vaultState.midPosition = new float3(position.x, ledgePosition.y, position.z) + (math.up() * .1f);
                vaultState.endPosition = AdjustDestinationHeight(collisionWorld, collisionFilter, vaultSettings.maxTopHeightRange, ledgePosition, ledgePosition + forward * vaultSettings.destinationOffset) + (math.up() * .1f);
                return true;
            }
        }
        vaultState.midPosition = vaultState.endPosition = position;
        return false;
    }

    /// <summary>
    /// Get the position of a vaultable wall in front of the player.
    /// </summary>
    /// <param name="collisionWorld">The collision world for raycasting</param>
    /// <param name="collisionFilter">The filter used for the raycast</param>
    /// <param name="playerPosition">Players position</param>
    /// <param name="playerForward">Players forward direction</param>
    /// <param name="vaultSettings">settings needed to define a valid vaultable object</param>
    /// <param name="wallPosition">The position is set to the hit position of a valid wall</param>
    /// <returns>True when a valid vaultable wall is hit</returns>
    static bool GetVaultPosition(in CollisionWorld collisionWorld, in CollisionFilter collisionFilter,
        in float3 playerPosition, in float3 playerForward, in AbilityVaultSettings vaultSettings, out float3 wallPosition)
    {
        for (float i = vaultSettings.detectionHeightRange.x; i < vaultSettings.detectionHeightRange.y; i += vaultSettings.detectionSpacing)
        {
            var start = playerPosition + (math.up() * i);
            var end = start + (playerForward * vaultSettings.detectionDepthRange);

            RaycastInput rayInput = new RaycastInput
            {
                Start = start,
                End = end,
                Filter = collisionFilter
            };
            if (collisionWorld.CastRay(rayInput, out var hitInfo))
            {
                var normal = math.abs(hitInfo.SurfaceNormal.y);
                if (normal <= vaultSettings.maxWallSteepnessCosine + math.EPSILON)
                {
                    wallPosition = hitInfo.Position;
                    return true;
                }
            }
        }

        wallPosition = float3.zero;
        return false;
    }

    /// <summary>
    /// Get the height of the vaultable wall.
    /// </summary>
    /// <param name="collisionWorld">The collision world for raycasting</param>
    /// <param name="collisionFilter">The filter used for the raycast</param>
    /// <param name="wallPosition">Vaultable wall position</param>
    /// <param name="playerPosition">Players position</param>
    /// <param name="playerForward">Players forward direction</param>
    /// <param name="vaultSettings">settings needed to define a valid vaultable object</param>
    /// <param name="ledgePosition">The ledge height is set to the hit position</param>
    /// <returns>True when the height of the wall is between the height range</returns>
    static bool GetVaultHeight(in CollisionWorld collisionWorld, in CollisionFilter collisionFilter, in float3 wallPosition,
        in float3 playerPosition, in float3 playerForward, in AbilityVaultSettings vaultSettings, out float3 ledgePosition)
    {
        ledgePosition = float3.zero;
        var pos = new float3(wallPosition.x, playerPosition.y, wallPosition.z);
        var wallThicknessPosition = pos + (playerForward * vaultSettings.minimumWallThickness);

        var distance = vaultSettings.detectionHeightRange.y + 1f; // +1 so it can also hit a target at the maximum range
        RaycastInput rayInput = new RaycastInput
        {
            Start = wallThicknessPosition + (math.up() * distance),
            End = wallThicknessPosition - (math.up() * distance),
            Filter = collisionFilter
        };

        if (collisionWorld.CastRay(rayInput, out var hit))
        {
            ledgePosition = new float3(wallPosition.x, hit.Position.y, wallPosition.z);
            var relativeHeight = hit.Position.y - playerPosition.y;
            return relativeHeight >= vaultSettings.detectionHeightRange.x && relativeHeight <= vaultSettings.detectionHeightRange.y;
        }

        return false;
    }

    public static float3 AdjustDestinationHeight(in CollisionWorld collisionWorld, in CollisionFilter collisionFilter, in float destinationThresshold, in float3 ledge, in float3 destination)
    {
        RaycastInput rayInput = new RaycastInput
        {
            Start = destination + (math.up() * destinationThresshold),
            End = destination - (math.up() * .1f),
            Filter = collisionFilter
        };

        if (collisionWorld.CastRay(rayInput, out var hit)
            && hit.Position.y > destination.y
            && (hit.Position.y - destination.y) <= destinationThresshold)
        {
            var adjustedDirection = math.normalize(hit.Position - ledge);
            return ledge + (adjustedDirection * math.length(ledge - destination));
        }
        return destination;
    }

    /// <summary>
    /// Check if the vault path is clear of colliders.
    /// </summary>
    /// <param name="collisionWorld">The collision world for collider casting</param>
    /// <param name="collider">Collider of the player for the collider cast</param>
    /// <param name="entity">Exclude entity from the collider cast</param>
    /// <param name="start">Start position of the vault</param>
    /// <param name="mid">Middle position of the vault</param>
    /// <param name="end">End position of the vault</param>
    /// <returns>True when no other collisions have been found in the vault path</returns>
    public static bool IsPathValid(in CollisionWorld collisionWorld, in BlobAssetReference<Collider> collider, in Entity entity, in float3 start, in float3 mid, in float3 end)
    {
        // Check collision from startpoint to midpoint
        if (collisionWorld.CastCollider(new ColliderCastInput(collider, start, mid), out var closestHit)
            && closestHit.Entity != entity)
        {
            return false; // Hit something that isn't the player
        }

        // Check collision from midpoint to endpoint
        if (collisionWorld.CastCollider(new ColliderCastInput(collider, mid, end), out closestHit)
            && closestHit.Entity != entity)
        {
            return false; // Hit something that isn't the player
        }

        return true;
    }

    /// <summary>
    /// Log and draw debug lines of the wall height
    /// </summary>
    /// <param name="collisionWorld">The collision world for raycasting</param>
    /// <param name="collisionFilter">The filter used for the raycast</param>
    /// <param name="ledgePosition">The top ledge position of the wall that is set at GetVaultHeight</param>
    /// <param name="playerPosition">The players position</param>
    static void DebugWallHeight(in CollisionWorld collisionWorld, in CollisionFilter collisionFilter, in float3 ledgePosition, in float3 playerPosition)
    {
        var player = new float3(playerPosition.x, ledgePosition.y, playerPosition.z);
        var start = (ledgePosition + player) * .5f;
        var end = start - (math.up() * 10);

        RaycastInput rayInput = new RaycastInput
        {
            Start = start,
            End = end,
            Filter = collisionFilter
        };
        if (collisionWorld.CastRay(rayInput, out var hit))
        {
            DebugDraw.Sphere(start, .1f, UnityEngine.Color.cyan, 1);
            DebugDraw.Sphere(end, .1f, UnityEngine.Color.cyan, 1);
            DebugDraw.Line(start, end, UnityEngine.Color.magenta, 1);
            UnityEngine.Debug.Log($"Wall height: {math.abs(hit.Position.y - start.y)}");
        }
    }
    #endregion

    #region Vault Derived Data
    /// <summary>
    /// Returns the completion of the upward part as a scalar
    /// </summary>
    public static float CalculateUpwardProgression(in GameTime gameTime, in uint startTick, in float upwardDuration)
    {
        return math.saturate(gameTime.DurationSinceTickNoFraction(startTick) / upwardDuration);
    }

    /// <summary>
    /// Returns the completion of the forward part as a scalar
    /// </summary>
    public static float CalculateForwardProgression(in GameTime gameTime, in uint startTick, in float upwardsDuration, in float curveDuration)
    {
        return math.saturate((gameTime.DurationSinceTickNoFraction(startTick) - upwardsDuration) / curveDuration);
    }
    #endregion
}
