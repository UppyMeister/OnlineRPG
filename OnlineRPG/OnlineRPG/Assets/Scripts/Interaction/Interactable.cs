﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public abstract class Interactable : MonoBehaviour
{
    #region Delegates
    public delegate void OnInteractableReachedDelegate();
    #endregion

    [Header("Base Class Variables")]
    public Transform interactionTransform;
    public float radius = 3f;

    [Space]

    [Header("Cursor")]
    public Texture2D defaultCursor;
    public Texture2D hoverCursor;

    [Space]

    [Tooltip("The name of the interactable.")] public string interactableName;

    bool isMoving = false;
    bool hasArrived = false;

    OnInteractableReachedDelegate onInteractableReached;

    [HideInInspector] public Player player;
    // Every interactable will have these options.
    // The class that derives from interactable SHOULD (but is not required to) use these as well as their own.
    [HideInInspector] public List<Option> baseInteractOptions;

    void Update()
    {
        if (isMoving && !hasArrived)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= radius)
            {
                if (onInteractableReached != null)
                {
                    onInteractableReached.Invoke();
                }

                hasArrived = true;
                isMoving = false;
            }
        }
    }

    public void InitializeOptions()
    {
        baseInteractOptions = new List<Option>()
        {
            new Option()
            {
                Text = "Walk here",
                OnOptionClick = WalkToInteractable
            }
        };
    }

    public void WalkToInteractable()
    {
        player.MoveToPoint(transform.position);
    }

    public void FollowInteractable()
    {
        // TODO.
    }

    public void OnFocus(Player player)
    {
        this.player = player;
    }

    public void OnDefocus()
    {
        this.player = null;
        isMoving = false;
        hasArrived = false;
    }

    public void MoveToInteractable(OnInteractableReachedDelegate onInteractableReached)
    {
        isMoving = true;
        this.onInteractableReached = onInteractableReached;
        hasArrived = false;

        if (player != null)
        {
            player.GetComponent<Player>().FollowTarget(this);
        }
    }

    #region Gizmos
    void OnDrawGizmosSelected()
    {
        if (interactionTransform == null)
        {
            interactionTransform = this.transform;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }
    #endregion

    #region Cursor
    private void OnMouseOver()
    {
        if (gameObject == Player.LocalPlayer) return;

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.ForceSoftware);
        } else
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
        }
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }
    #endregion
}