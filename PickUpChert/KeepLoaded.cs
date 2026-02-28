//using NewHorizons.Builder.General;
//using NewHorizons.Builder.Props;
//using NewHorizons.Components;
//using NewHorizons.Components.Orbital;
//using NewHorizons.Components.Props;
//using NewHorizons.External.Modules.Props;
//using NewHorizons.Handlers;
//using NewHorizons.Utility;
//using NewHorizons.Utility.OuterWilds;
//using NewHorizons.Utility.OWML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PickUpChert {
    public static class KeepLoaded {
        // from https://github.com/Outer-Wilds-New-Horizons/new-horizons/blob/1efcece10b526ad160370e9ba7d802b9023beef3/NewHorizons/Builder/Props/DetailBuilder.cs#L110
        public static void MakeKeepLoaded(GameObject prop, bool hasPhysics) {
            //GameObject prop;
            bool isItem;
            bool invalidComponentFound = false;
            //bool isFromAssetBundle = !string.IsNullOrEmpty(detail.assetBundle);
            bool isFromAssetBundle = false;


            //// Reflection
            //Type detailBuilderType = typeof(DetailBuilder);
            //MethodInfo fixComponent = detailBuilderType.GetMethod("FixComponent");

            // We save copies with all their components fixed, good if the user is placing the same detail more than once
            //if (detail?.path != null && _fixedPrefabCache.TryGetValue((sector, detail.path, detail.sectorPath), out var storedPrefab)) {
            //    prop = GeneralPropBuilder.MakeFromPrefab(storedPrefab.prefab, prefab.name, go, ref sector, detail);
            //    isItem = storedPrefab.isItem;
            //}
            //else {
                //prop = GeneralPropBuilder.MakeFromPrefab(prefab, prefab.name, go, ref sector, detail);

                //StreamingHandler.SetUpStreaming(prop, detail.keepLoaded ? null : sector);
                //StreamingHandler.SetUpStreaming(prop, null);
                PickUpChert.NHAssembly._streamingHandlerSetUpStreaming.Invoke(null, new object[] { prop, null });

                // Could check this in the for loop but I'm not sure what order we need to know about this in
                isItem = false;

                var existingSectors = new HashSet<Sector>(prop.GetComponentsInChildren<Sector>(true));

                foreach (var component in prop.GetComponentsInChildren<Component>(true)) {
                    // Rather than having the entire prop not exist when a detail fails let's just try-catch and log an error
                    //try {
                        // Components can come through as null here (yes, really),
                        // Usually if a script was added to a prefab in an asset bundle but isn't present in the loaded mod DLLs
                        if (component == null) {
                            invalidComponentFound = true;
                            continue;
                        }
                        //if (component.gameObject == prop && component is OWItem) isItem = true;

                        //if (sector == null) {
                            if (FixUnsectoredComponent(component)) continue;
                        //}
                        //else {
                        //    // Fix cull groups only when not from an asset bundle (because then they're there on purpose!)
                        //    // keepLoaded should remove existing groups
                        //    // renderers/colliders get enabled later so we dont have to do that here
                        //    if (detail.keepLoaded && !isFromAssetBundle && component is SectorCullGroup or SectorCollisionGroup or SectorLightsCullGroup) {
                        //        UnityEngine.Object.DestroyImmediate(component);
                        //        continue;
                        //    }

                        //    /* We used to set SectorCullGroup._controllingProxy to null. Now we do not.
                        //     * This may break things on copied details because it prevents SetSector from doing anything,
                        //     * so that part of the detail might be culled by wrong sector.
                        //     * So if you copy something from e.g. Giants Deep it might turn off the detail if you arent in 
                        //     * the sector of the thing you copied from (since it's still pointing to the original proxy, 
                        //     * which has the original sector at giants deep there)
                        //     * 
                        //     * Anyway nobody has complained about this for the year it's been like that so closing issue #831 until
                        //     * this affects somebody
                        //     */

                        //    FixSectoredComponent(component, sector, existingSectors);
                        //}

                        // Asset bundle is a real string -> Object loaded from unity
                        // If they're adding dialogue we have to manually register the xml text
                        //if (isFromAssetBundle && component is CharacterDialogueTree dialogue) {
                        //    DialogueBuilder.HandleUnityCreatedDialogue(dialogue);
                        //}

                        // copied details need their lanterns fixed
                        if (!isFromAssetBundle && component is DreamLanternController lantern) {
                            //lantern.gameObject.AddComponent<DreamLanternControllerFixer>();
                            lantern.gameObject.AddComponent(PickUpChert.NHAssembly._dreamLanternControllerFixer);
                        }

                        //FixComponent(component, go, detail.ignoreSun);
                        //FixComponent(component, null, false);
                        PickUpChert.NHAssembly._detailBuilderFixComponent.Invoke(null, new object[] { component, null, false });
                        //fixComponent.Invoke(null, new object[] { component, null, false });
                    //}
                    //catch (Exception e) {
                    //    NHLogger.LogError($"Failed to correct component {component?.GetType()?.Name} on {go?.name} - {e}");
                    //}
                }

                //if (detail.path != null) {
                //    // We put these in DontDestroyOnLoad so that QSB will ignore them and so they don't clutter up the scene.
                //    _fixedPrefabCache.Add((sector, detail.path, detail.sectorPath), (prop.InstantiateInactive().DontDestroyOnLoad(), isItem));
                //}
            //}

            //if (invalidComponentFound) {
            //    foreach (Transform t in prop.GetComponentsInChildren<Transform>(true)) {
            //        if (t.GetComponents<Component>().Any(c => c == null)) {
            //            NHLogger.LogError($"Failed to instantiate component at {t.GetPath()}. This usually means there's a missing script.");
            //        }
            //    }
            //}

            //prop.transform.localScale = detail.stretch ?? (detail.scale != 0 ? Vector3.one * detail.scale : prefab.transform.localScale);

            //if (detail.removeChildren != null) {
            //    foreach (var childPath in detail.removeChildren) {
            //        var flag = true;
            //        foreach (var childObj in prop.transform.FindAll(childPath)) {
            //            flag = false;
            //            childObj.gameObject.SetActive(false);
            //        }

            //        if (flag) NHLogger.LogWarning($"Couldn't find \"{childPath}\".");
            //    }
            //}

            //if (detail.removeComponents) {
            //    NHLogger.LogVerbose($"Removing all components from [{prop.name}]");

            //    // Just swap all the children to a new game object
            //    var newDetailGO = new GameObject(prop.name);
            //    newDetailGO.SetActive(false);
            //    newDetailGO.transform.parent = prop.transform.parent;
            //    newDetailGO.transform.position = prop.transform.position;
            //    newDetailGO.transform.rotation = prop.transform.rotation;
            //    newDetailGO.transform.localScale = prop.transform.localScale;

            //    // Can't modify parents while looping through children bc idk
            //    var children = new List<Transform>();
            //    foreach (Transform child in prop.transform) {
            //        children.Add(child);
            //    }
            //    foreach (var child in children) {
            //        child.parent = newDetailGO.transform;
            //    }
            //    // Have to destroy it right away, else parented props might get attached to the old one
            //    UnityEngine.Object.DestroyImmediate(prop);
            //    prop = newDetailGO;
            //}

            //if (detail.item != null) {
            //    ItemBuilder.MakeItem(prop, go, sector, detail.item, mod);
            //    isItem = true;
            //    if (detail.hasPhysics) {
            //        NHLogger.LogWarning($"An item with the path {detail.path} has both '{nameof(DetailInfo.hasPhysics)}' and '{nameof(DetailInfo.item)}' set. This will usually result in undesirable behavior.");
            //    }
            //}

            //if (detail.itemSocket != null) {
            //    ItemBuilder.MakeSocket(prop, go, sector, detail.itemSocket);
            //}

            //if (isItem) {
            //    // Else when you put them down you can't pick them back up
            //    var col = prop.GetComponent<OWCollider>();
            //    if (col != null) col._physicsRemoved = false;

            //    // Items should always be kept loaded else they will vanish in your hand as you leave the sector
            //    detail.keepLoaded = true;
            //}

            //if (!detail.keepLoaded) GroupsBuilder.Make(prop, sector);

            //// For DLC related props
            //// Make sure to do this before its set active
            //if (!string.IsNullOrEmpty(detail?.path) &&
            //    (detail.path.ToLowerInvariant().StartsWith("ringworld") || detail.path.ToLowerInvariant().StartsWith("dreamworld"))) {
            //    prop.AddComponent<DestroyOnDLC>()._destroyOnDLCNotOwned = true;
            //}

            prop.SetActive(true);

            //if (detail.hasPhysics) {
            if (hasPhysics) {
                //var addPhysics = prop.AddComponent<AddPhysics>();
                var addPhysics = prop.AddComponent(PickUpChert.NHAssembly._addPhysics);
                //addPhysics.Sector = detail.keepLoaded ? null : sector;
                //addPhysics.Sector = null;
                //addPhysics.Mass = 1;//detail.physicsMass;
                //addPhysics.Radius = 1;// detail.physicsRadius;
                //addPhysics.SuspendUntilImpact = false;//detail.physicsSuspendUntilImpact;
            }

            //if (!string.IsNullOrEmpty(detail.activationCondition)) {
            //    ConditionalObjectActivation.SetUp(prop, detail.activationCondition, detail.blinkWhenActiveChanged, true);
            //}
            //if (!string.IsNullOrEmpty(detail.deactivationCondition)) {
            //    ConditionalObjectActivation.SetUp(prop, detail.deactivationCondition, detail.blinkWhenActiveChanged, false);
            //}

            //_detailInfoToGameObject[detail] = prop;

            //return prop;
        }

        private static bool FixUnsectoredComponent(Component component) {
            if (component is FogLight or SectoredMonoBehaviour or ISectorGroup) {
                UnityEngine.Object.DestroyImmediate(component);
                return true;
            }
            return false;
        }

        //private static void FixComponent(Component component, GameObject planetGO, bool ignoreSun) {
        //    // Fix other components
        //    if (component is Transform) {
        //        if (!ignoreSun && component.gameObject.layer == Layer.IgnoreSun) {
        //            component.gameObject.layer = Layer.Default;
        //        }
        //        else if (ignoreSun && component.gameObject.layer == Layer.Default) {
        //            component.gameObject.layer = Layer.IgnoreSun;
        //        }
        //    }
        //    else if (component is DarkMatterVolume) {
        //        var probeVisuals = component.gameObject.transform.Find("ProbeVisuals");
        //        if (probeVisuals != null) probeVisuals.gameObject.SetActive(true);
        //    }
        //    else if (component is DarkMatterSubmergeController submergeController) {
        //        var water = planetGO.GetComponentsInChildren<RadialFluidVolume>().FirstOrDefault(x => x._fluidType == FluidVolume.Type.WATER);
        //        if (submergeController._fluidDetector)
        //            submergeController._fluidDetector._onlyDetectableFluid = water;
        //    }
        //    // Fix anglerfish speed on orbiting planets
        //    else if (component is AnglerfishController angler) {
        //        if (planetGO?.GetComponent<NHAstroObject>() is NHAstroObject nhao && !nhao.invulnerableToSun) {
        //            // Has a fluid detector, will go gorp (#830)
        //            NHLogger.LogWarning("Having an anglerfish on a planet that has a fluid detector can lead to things breaking!");
        //        }

        //        try {
        //            angler._chaseSpeed += OWPhysics.CalculateOrbitVelocity(planetGO.GetAttachedOWRigidbody(), planetGO.GetComponent<AstroObject>().GetPrimaryBody().GetAttachedOWRigidbody()).magnitude;
        //        }
        //        catch (Exception e) {
        //            NHLogger.LogError($"Couldn't update AnglerFish chase speed:\n{e}");
        //        }
        //    }

        //    // fix campfires
        //    else if (component is InteractVolume) {
        //        component.gameObject.AddComponent<InteractVolumeFixer>();
        //    }
        //    else if (component is PlayerAttachPoint) {
        //        component.gameObject.AddComponent<PlayerAttachPointFixer>();
        //    }

        //    else if (component is NomaiInterfaceOrb orb) {
        //        // detect planet gravity
        //        // somehow Intervention has GetAttachedOWRigidbody as null sometimes, idk why
        //        var gravityVolume = planetGO.GetAttachedOWRigidbody()?.GetAttachedGravityVolume();
        //        orb.GetComponent<ConstantForceDetector>()._detectableFields = gravityVolume ? new ForceVolume[] { gravityVolume } : new ForceVolume[0];
        //    }

        //    else if (component is VisionTorchItem torchItem) {
        //        torchItem.enabled = true;
        //        torchItem.mindProjectorTrigger.enabled = true;
        //        torchItem.gameObject.AddComponent<VisionTorchItemFixer>();
        //    }

        //    else if (component is Animator animator) animator.enabled = true;
        //    else if (component is Collider collider) collider.enabled = true;
        //    // Bug 533 - Don't show the electricity effect renderers
        //    else if (component is Renderer renderer && component.gameObject.GetComponent<ElectricityEffect>() == null) renderer.enabled = true;
        //    else if (component is Shape shape) shape.enabled = true;

        //    // If it's not a moving ghostbird (ie Prefab_IP_GhostBird/Ghostbird_IP_ANIM) make sure it doesnt spam NREs
        //    // Manual parent chain so we can find inactive
        //    else if (component is GhostIK or GhostEffects && component.transform.parent.GetComponent<GhostBrain>() == null) {
        //        UnityEngine.Object.DestroyImmediate(component);
        //    }
        //    // If it's not a moving anglerfish (ie Anglerfish_Body/Beast_Anglerfish) make sure the anim controller is regular
        //    // Manual parent chain so we can find inactive
        //    else if (component is AnglerfishAnimController && component.transform.parent.GetComponent<AnglerfishController>() == null) {
        //        component.gameObject.AddComponent<AnglerAnimFixer>();
        //    }
        //    // Add custom logic to NH-spawned rafts to handle fluid changes
        //    else if (component is RaftController raft) {
        //        component.gameObject.AddComponent<NHRaftController>();
        //    }
        //    else if (component is RaftDock dock) {
        //        // These flood toggles are to disable flooded docks on the Stranger
        //        // Presumably the user isn't making one of those
        //        foreach (var toggle in dock.GetComponentsInChildren<FloodToggle>()) {
        //            Component.DestroyImmediate(toggle);
        //        }
        //        foreach (var floodSensor in dock.GetComponentsInChildren<RingRiverFloodSensor>()) {
        //            Component.DestroyImmediate(floodSensor);
        //        }
        //    }
        //}

        //[RequireComponent(typeof(AnglerfishAnimController))]
        //private class AnglerAnimFixer : MonoBehaviour {
        //    public void Start() {
        //        var angler = GetComponent<AnglerfishAnimController>();

        //        NHLogger.LogVerbose("Fixing anglerfish animation");

        //        // Remove any event reference to its angler so that they dont change its state
        //        if (angler._anglerfishController) {
        //            angler._anglerfishController.OnChangeAnglerState -= angler.OnChangeAnglerState;
        //            angler._anglerfishController.OnAnglerTurn -= angler.OnAnglerTurn;
        //            angler._anglerfishController.OnAnglerSuspended -= angler.OnAnglerSuspended;
        //            angler._anglerfishController.OnAnglerUnsuspended -= angler.OnAnglerUnsuspended;
        //        }
        //        // Disable the angler anim controller because we don't want Update or LateUpdate to run, just need it to set the initial Animator state
        //        angler.enabled = false;
        //        angler.OnChangeAnglerState(AnglerfishController.AnglerState.Lurking);

        //        angler._animator.SetFloat("MoveSpeed", angler._moveCurrent);
        //        angler._animator.SetFloat("Jaw", angler._jawCurrent);

        //        Destroy(this);
        //    }
        //}

        ///// <summary>
        ///// Has to happen after 1 frame to work with VR
        ///// </summary>
        //[RequireComponent(typeof(InteractVolume))]
        //private class InteractVolumeFixer : MonoBehaviour {
        //    public void Start() {
        //        var interactVolume = GetComponent<InteractVolume>();
        //        interactVolume._playerCam = Locator.GetPlayerCamera();

        //        Destroy(this);
        //    }
        //}

        ///// <summary>
        ///// Has to happen after 1 frame to work with VR
        ///// </summary>
        //[RequireComponent(typeof(PlayerAttachPoint))]
        //private class PlayerAttachPointFixer : MonoBehaviour {
        //    public void Start() {
        //        var playerAttachPoint = GetComponent<PlayerAttachPoint>();
        //        playerAttachPoint._playerController = Locator.GetPlayerController();
        //        playerAttachPoint._playerOWRigidbody = Locator.GetPlayerBody();
        //        playerAttachPoint._playerTransform = Locator.GetPlayerTransform();
        //        playerAttachPoint._fpsCamController = Locator.GetPlayerCameraController();

        //        Destroy(this);
        //    }
        //}

        ///// <summary>
        ///// Has to happen after 1 frame to work with VR
        ///// </summary>
        //[RequireComponent(typeof(VisionTorchItem))]
        //private class VisionTorchItemFixer : MonoBehaviour {
        //    public void Start() {
        //        var torchItem = GetComponent<VisionTorchItem>();
        //        torchItem.mindSlideProjector._mindProjectorImageEffect = Locator.GetPlayerCamera().GetComponent<MindProjectorImageEffect>();

        //        Destroy(this);
        //    }
        //}

        //[RequireComponent(typeof(DreamLanternController))]
        //private class DreamLanternControllerFixer : MonoBehaviour {
        //    private void Start() {
        //        // based on https://github.com/Bwc9876/OW-Amogus/blob/master/Amogus/LanternCreator.cs
        //        // needed to fix petals looking backwards, among other things

        //        var lantern = GetComponent<DreamLanternController>();

        //        // this is set in Awake, we wanna override it

        //        // Manually copied these values from a artifact lantern so that we don't have to find it (works in Eye)
        //        lantern._origLensFlareBrightness = 0f;
        //        lantern._focuserPetalsBaseEulerAngles = new Vector3[]
        //        {
        //            new Vector3(0.7f, 270.0f, 357.5f),
        //            new Vector3(288.7f, 270.1f, 357.4f),
        //            new Vector3(323.3f, 90.0f, 177.5f),
        //            new Vector3(35.3f, 90.0f, 177.5f),
        //            new Vector3(72.7f, 270.1f, 357.5f)
        //        };
        //        lantern._dirtyFlag_focus = true;
        //        lantern._concealerRootsBaseScale = new Vector3[]
        //        {
        //            Vector3.one,
        //            Vector3.one,
        //            Vector3.one
        //        };
        //        lantern._concealerCoversStartPos = new Vector3[]
        //        {
        //            new Vector3(0.0f, 0.0f, 0.0f),
        //            new Vector3(0.0f, -0.1f, 0.0f),
        //            new Vector3(0.0f, -0.2f, 0.0f),
        //            new Vector3(0.0f, 0.2f, 0.0f),
        //            new Vector3(0.0f, 0.1f, 0.0f),
        //            new Vector3(0.0f, 0.0f, 0.0f)
        //        };
        //        lantern._dirtyFlag_concealment = true;
        //        lantern.UpdateVisuals();

        //        Destroy(this);
        //    }
        //}
    }
}
