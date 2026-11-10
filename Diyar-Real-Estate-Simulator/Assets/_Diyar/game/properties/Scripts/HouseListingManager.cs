using System.Collections.Generic;
using UnityEngine;

namespace Diyar.Game
{
    public sealed class HouseListingManager : MonoBehaviour
    {
        private const int MinimumPrice = 25000;
        private const int MaximumPrice = 90000;

        private PlayerInteraction playerInteraction;
        private PlayerWallet playerWallet;
        private Material postMaterial;
        private Material forSaleMaterial;
        private Material soldMaterial;

        private void Start()
        {
            playerInteraction = FindObjectOfType<PlayerInteraction>();
            playerWallet = FindObjectOfType<PlayerWallet>();

            if (playerInteraction == null || playerWallet == null)
            {
                Debug.LogError("Hausangebote brauchen PlayerInteraction und PlayerWallet.", this);
                return;
            }

            CreateMaterials();

            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
            int listingNumber = 0;

            foreach (Transform house in transform)
            {
                Renderer[] renderers = house.GetComponentsInChildren<Renderer>();

                if (renderers.Length == 0)
                {
                    continue;
                }

                Bounds houseBounds = renderers[0].bounds;

                for (int i = 1; i < renderers.Length; i++)
                {
                    houseBounds.Encapsulate(renderers[i].bounds);
                }

                listingNumber++;

                if (!TryInitializePlacedSign(house, listingNumber))
                {
                    CreateListing(house, houseBounds, listingNumber, playerMovement);
                }
            }
        }

        private void CreateListing(
            Transform house,
            Bounds houseBounds,
            int listingNumber,
            PlayerMovement playerMovement)
        {
            string propertyId = $"damascus_{house.name.ToLowerInvariant().Replace(' ', '_')}_{listingNumber:00}";
            int price = GetStableRandomPrice(propertyId);
            Vector3 frontDirection = house.forward;
            Vector3 signPosition = houseBounds.center + frontDirection * (houseBounds.extents.z + 2f);

            if (TryGetFrontDoorBounds(house, houseBounds, frontDirection, out Bounds doorBounds))
            {
                Vector3 doorOffset = Vector3.ProjectOnPlane(
                    doorBounds.center - houseBounds.center,
                    Vector3.up);

                if (doorOffset.sqrMagnitude > 0.25f)
                {
                    frontDirection = doorOffset.normalized;
                }

                float doorDepth = Mathf.Abs(frontDirection.x) * doorBounds.extents.x
                    + Mathf.Abs(frontDirection.z) * doorBounds.extents.z;
                Vector3 sideDirection = Vector3.Cross(Vector3.up, frontDirection).normalized;
                signPosition = doorBounds.center
                    + frontDirection * (doorDepth + 1.1f)
                    + sideDirection * 0.85f;
            }

            signPosition.y = houseBounds.min.y;

            Vector3 signFacing = frontDirection;

            if (playerMovement != null)
            {
                Vector3 directionToPlayer = Vector3.ProjectOnPlane(
                    playerMovement.transform.position - signPosition,
                    Vector3.up);

                if (directionToPlayer.sqrMagnitude > 0.25f)
                {
                    signFacing = directionToPlayer.normalized;
                }
            }

            GameObject signRoot = new GameObject($"ForSaleSign_{listingNumber:00}");
            signRoot.transform.SetParent(house, true);
            signRoot.transform.SetPositionAndRotation(
                signPosition,
                Quaternion.LookRotation(signFacing, Vector3.up));

            BoxCollider signCollider = signRoot.AddComponent<BoxCollider>();
            signCollider.center = new Vector3(0f, 1.05f, 0f);
            signCollider.size = new Vector3(1.8f, 2.1f, 0.4f);

            CreateSignPart(signRoot.transform, "Post", new Vector3(0f, 0.85f, 0f),
                new Vector3(0.13f, 1.7f, 0.13f), postMaterial);
            GameObject board = CreateSignPart(signRoot.transform, "SignBoard",
                new Vector3(0f, 1.75f, 0f), new Vector3(1.55f, 0.5f, 0.12f), forSaleMaterial);

            GameObject labelObject = new GameObject("SignText");
            labelObject.transform.SetParent(signRoot.transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 1.75f, 0.07f);
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = "FOR SALE";
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 100;
            label.characterSize = 0.0025f;
            label.fontStyle = FontStyle.Bold;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = new Color(0.98f, 0.94f, 0.84f);

            HouseListing listing = signRoot.AddComponent<HouseListing>();
            listing.Initialize(propertyId, house.name, price, playerWallet, label,
                board.GetComponent<Renderer>(), forSaleMaterial, soldMaterial);
            playerInteraction.RegisterTarget(signCollider);
        }

        private bool TryInitializePlacedSign(Transform house, int listingNumber)
        {
            Transform signTransform = house.Find("ForSaleSign");

            if (signTransform == null)
            {
                return false;
            }

            BoxCollider signCollider = signTransform.GetComponent<BoxCollider>();

            if (signCollider == null)
            {
                signCollider = signTransform.gameObject.AddComponent<BoxCollider>();
                signCollider.center = new Vector3(0.5f, 1f, 0f);
                signCollider.size = new Vector3(1.2f, 2f, 0.25f);
            }

            Renderer[] signRenderers = signTransform.GetComponentsInChildren<Renderer>(true);
            Renderer boardRenderer = null;
            int boardMaterialIndex = -1;
            Material[] boardMaterials = null;

            foreach (Renderer candidate in signRenderers)
            {
                Material[] candidateMaterials = candidate.sharedMaterials;

                for (int i = 0; i < candidateMaterials.Length; i++)
                {
                    Material candidateMaterial = candidateMaterials[i];
                    bool isSaleMaterial = candidateMaterial != null
                        && (candidateMaterial.name.Contains("SignMat")
                            || (candidateMaterial.mainTexture != null
                                && candidateMaterial.mainTexture.name.Contains("sign_texture")));

                    if (isSaleMaterial)
                    {
                        boardRenderer = candidate;
                        boardMaterialIndex = i;
                        boardMaterials = candidateMaterials;
                        break;
                    }
                }

                if (boardRenderer != null)
                {
                    break;
                }

                if (candidate.gameObject.name.Contains("Sign"))
                {
                    boardRenderer = candidate;
                    boardMaterialIndex = 0;
                    boardMaterials = candidateMaterials;
                }
            }

            if (boardRenderer == null && signRenderers.Length == 1)
            {
                boardRenderer = signRenderers[0];
                boardMaterials = boardRenderer.sharedMaterials;
                boardMaterialIndex = boardMaterials.Length - 1;
            }

            if (boardRenderer == null || boardMaterials == null || boardMaterialIndex < 0
                || boardMaterialIndex >= boardMaterials.Length)
            {
                Debug.LogWarning($"ForSaleSign unter {house.name} enthält kein Material SignMat oder sign_texture.", signTransform);
                return false;
            }

            TextMesh signLabel = signTransform.GetComponentInChildren<TextMesh>(true);

            if (signLabel == null)
            {
                GameObject labelObject = new GameObject("StatusText");
                labelObject.transform.SetParent(signTransform, false);
                labelObject.transform.localPosition = new Vector3(0.535f, 1.475f, 0.062f);
                signLabel = labelObject.AddComponent<TextMesh>();
                signLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                signLabel.fontSize = 90;
                signLabel.characterSize = 0.0025f;
                signLabel.fontStyle = FontStyle.Bold;
                signLabel.anchor = TextAnchor.MiddleCenter;
                signLabel.alignment = TextAlignment.Center;
                signLabel.color = Color.white;
            }

            string propertyId = $"damascus_{house.name.ToLowerInvariant().Replace(' ', '_')}_{listingNumber:00}";
            int price = GetStableRandomPrice(propertyId);
            HouseListing listing = signTransform.GetComponent<HouseListing>();

            if (listing == null)
            {
                listing = signTransform.gameObject.AddComponent<HouseListing>();
            }

            Material saleMaterial = boardMaterials[boardMaterialIndex];
            listing.Initialize(propertyId, house.name, price, playerWallet, signLabel,
                boardRenderer, saleMaterial, soldMaterial, true, boardMaterialIndex);
            playerInteraction.RegisterTarget(signCollider);
            return true;
        }

        private bool TryGetFrontDoorBounds(
            Transform house,
            Bounds houseBounds,
            Vector3 frontDirection,
            out Bounds frontDoorBounds)
        {
            List<Renderer> doorRenderers = new List<Renderer>();

            foreach (Renderer renderer in house.GetComponentsInChildren<Renderer>(true))
            {
                Transform current = renderer.transform;

                while (current != null && current != house)
                {
                    if (current.name == "Doors")
                    {
                        doorRenderers.Add(renderer);
                        break;
                    }

                    current = current.parent;
                }
            }

            if (doorRenderers.Count == 0)
            {
                frontDoorBounds = default;
                return false;
            }

            float lowestDoorHeight = float.PositiveInfinity;

            foreach (Renderer renderer in doorRenderers)
            {
                lowestDoorHeight = Mathf.Min(lowestDoorHeight, renderer.bounds.center.y);
            }

            Renderer frontDoor = null;
            float bestFrontPosition = float.NegativeInfinity;

            foreach (Renderer renderer in doorRenderers)
            {
                Bounds bounds = renderer.bounds;

                if (bounds.center.y > lowestDoorHeight + 1.5f)
                {
                    continue;
                }

                Vector3 horizontalOffset = Vector3.ProjectOnPlane(
                    bounds.center - houseBounds.center,
                    Vector3.up);
                float frontPosition = Vector3.Dot(horizontalOffset, frontDirection);

                if (frontPosition > bestFrontPosition)
                {
                    frontDoor = renderer;
                    bestFrontPosition = frontPosition;
                }
            }

            if (frontDoor == null)
            {
                frontDoorBounds = default;
                return false;
            }

            frontDoorBounds = frontDoor.bounds;
            return true;
        }

        private GameObject CreateSignPart(
            Transform parent,
            string partName,
            Vector3 localPosition,
            Vector3 localScale,
            Material material)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = partName;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            part.GetComponent<Renderer>().sharedMaterial = material;
            Destroy(part.GetComponent<Collider>());

            return part;
        }

        private void CreateMaterials()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");

            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                Debug.LogError("Es wurde kein passender Shader für die Verkaufsschilder gefunden.", this);
                return;
            }

            postMaterial = CreateMaterial(shader, new Color(0.72f, 0.64f, 0.48f));
            forSaleMaterial = CreateMaterial(shader, new Color(0.61f, 0.17f, 0.13f));
            soldMaterial = CreateMaterial(shader, new Color(0.30f, 0.32f, 0.30f));
        }

        private Material CreateMaterial(Shader shader, Color color)
        {
            Material material = new Material(shader);
            material.color = color;
            return material;
        }

        private int GetStableRandomPrice(string propertyId)
        {
            uint hash = 2166136261u;

            foreach (char character in propertyId)
            {
                hash = (hash ^ character) * 16777619u;
            }

            return MinimumPrice + (int)(hash % (MaximumPrice - MinimumPrice + 1));
        }

        private void OnDestroy()
        {
            DestroyMaterial(postMaterial);
            DestroyMaterial(forSaleMaterial);
            DestroyMaterial(soldMaterial);
        }

        private void DestroyMaterial(Material material)
        {
            if (material != null)
            {
                Destroy(material);
            }
        }
    }
}
