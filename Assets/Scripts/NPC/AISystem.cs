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
    [HideInInspector] public QuestGiver QuestGiver;
    [SerializeField] public GameObject InteractText;

    [HideInInspector] public bool InteractionPossible;
    [HideInInspector] public bool IsInteracting;
    [HideInInspector] public Vector3 StartPos;
    [HideInInspector] public Quaternion StartAngle;

    private Quaternion _startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    private Quaternion _stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    #endregion
    private void Start()
    {
        DialogueManager = GetComponent<DialogueManager>();
        Player = GameObject.FindGameObjectWithTag("Player");
        QuestGiver = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<QuestGiver>();
        _questKeeper = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestKeeper>();

        InteractionPossible = true;
        SetState(new BeginState(this));
        InteractText.SetActive(false);

        StartPos = transform.position;
        StartAngle = transform.rotation;
    }

    private void Update()
    {
        var ObjectFound = checkEnvironment();

        if (ObjectFound != null)
        {
            if (InteractionPossible == true)
            {
                if (Vector3.Distance(transform.position, Player.transform.position) > 4f)
                {
                    StartCoroutine(State.Follow());
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

                InteractText.SetActive(false);
            }
        }
        else
        {
            StartCoroutine(State.Idle());
        }

        if (InteractionPossible == false)
        {
            InteractText.SetActive(false);

            if (this.transform.position != StartPos)
            {
                StartCoroutine(State.Return());
            }
        }

        if(DialogueManager.npc.ConversationFinished == true)
        {
            _questKeeper.UpdateQuest();
            DialogueManager.npc.ConversationFinished = false;
        }

        if(_questKeeper.Quest != null)
        {
            if (_questKeeper.Quest.IsActive == true)
            {
                for (int i = 0; i < DialogueManager.npc.Quests.Length; i++)
                {
                    if (DialogueManager.npc.Quests[i].Goal.npcToTalkTo.name == DialogueManager.npc.name)
                    {
                        InteractionPossible = true;
                    }
                    else
                    {
                        InteractionPossible = false;
                    }
                }            
            }
            else
            {
                InteractionPossible = true;
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
