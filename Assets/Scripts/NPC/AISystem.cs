﻿using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Made by: Jorrit Bos
public class AISystem : StateMachine
{
    #region Variables
    [HideInInspector] public GameObject Player;
    [SerializeField] public int FollowSpeed = 0;
    [SerializeField] public int CheckingRadius = 0;

    [HideInInspector] private QuestKeeper _questKeeper;

    [HideInInspector] public DialogueManager DialogueManager;
    [SerializeField] public NPCInformation NpcInfo;
    [HideInInspector] public QuestGiver QuestGiver;
    [SerializeField] public GameObject InteractIcon;

    [HideInInspector] public bool InteractionPossible;
    [HideInInspector] public bool IsInteracting;
    [HideInInspector] public Vector3 StartPos;
    [HideInInspector] public Quaternion StartAngle;

    private Quaternion _startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    private Quaternion _stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    #endregion

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        QuestGiver = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<QuestGiver>();
    }
    private void Start()
    {
        DialogueManager = DontDestroyUI.UIInstance.UIGameObjects[0].GetComponent<DialogueManager>();
        _questKeeper = Player.GetComponent<QuestKeeper>();

        InteractionPossible = true;
        SetState(new AIBehaviours(this));
        InteractIcon.SetActive(false);

        StartPos = transform.position;
        StartAngle = transform.rotation;
    }

    private void Update()
    {
        var ObjectFound = checkEnvironment();
        checkAvailability();

        if (ObjectFound != null)
        {
            if (InteractionPossible == true)
            {
                if (Vector3.Distance(transform.position, Player.transform.position) > 2f)
                {
                    StartCoroutine(State.Follow());
                }
                else if (Vector3.Distance(transform.position, Player.transform.position) > 20f && transform.position != StartPos)
                {
                    StartCoroutine(State.Return());
                }
            }
            if (Vector3.Distance(transform.position, Player.transform.position) < 10f)
            {
                if (InteractionPossible == true)
                {
                    StartCoroutine(State.Interact());
                }
                else
                {
                    StartCoroutine(State.Unavailable());
                }
            }
            else
            {
                InteractIcon.SetActive(false);
            }
        }
        else
        {
            StartCoroutine(State.Idle());
        }

        if (InteractionPossible == false)
        {
            InteractIcon.SetActive(false);

            if (this.transform.position != StartPos)
            {
                StartCoroutine(State.Return());
            }
        }
    }

    private void checkAvailability()
    {
        if (DialogueManager.Npc != null)
        {
            if (DialogueManager.Npc.ConversationFinished != true)
            {
                InteractionPossible = true;
            }
        }

        if (_questKeeper.Quest != null)
        {
            if (_questKeeper.Quest.IsActive == true)
            {
                for (int i = 0; i < DialogueManager.Npc.Quests.Length; i++)
                {
                    if (DialogueManager.Npc.Quests[i].Goal.npcToTalkTo.ConversationFinished == true)
                    {
                        _questKeeper.UpdateQuest();
                        InteractionPossible = false;
                    }
                    else
                    {
                        InteractionPossible = false;
                    }
                }
            }
        }
    }

    private Transform checkEnvironment()
    {
        RaycastHit hit;
        var angle = transform.rotation * _startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        for (var i = 0; i < 24; i++)
        {
            if (Physics.Raycast(pos, direction, out hit, CheckingRadius))
            {
                var player = hit.collider.GetComponent<ThirdPersonCharacterController>();
                if (player != null)
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
                    return player.transform;
                }
                else
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(pos, direction * CheckingRadius, Color.white);
            }
            direction = _stepAngle * direction;
        }
        return null;
    }
}
