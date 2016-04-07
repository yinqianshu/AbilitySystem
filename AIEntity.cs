﻿using UnityEngine;
using AbilitySystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*

    entity.AddAbilityModifier(new TagMatcher(tags), new AddStatusModifier(new WhirlwindStatus));
    entity.AddAbilityModifier(new AbilityNameMatcher(tags), new AddStatusModifier(new WhirlwindStatus));

    AddAbilityModifer(matcher, modifier) {
        abilities = abilityManager.get(matcher)
        abilities.Foreach(ability)
            modifier.apply(ability)
        modifiers.Add(modifier)

    }

    Update() {
        foreach modifier
            modifer.Update()
            if(modifier.shouldRemove) 
                abilities = modifer.matcher.get(abilityManager)
                abilities.foreach
                    modifier.Restore(ability)
    }

    AddAbility() {
        foreach modifier
            if(modifier.matcher.matches(ability) {
                modifier.apply(ability)
            }
    }

    class WhirlWind : AbilityModifier {
        
        void Init() {
            entity.OnStatusAdded()
            entity.OnDamageTaken()
            entity.OnManaSpent()    
            
            entity.OnAbilityUsed(tags, OnAbilityUsed)
                }

        entity.OnDamageTaken(Source ability, float damage) {
        }

        entity.OnDamageDealt(Source ability) {
            if(ability.target.hasTag(tags) && ability.hasTag(tag)) {
                ability.damage.Value += modifier
            }
         }

        void OnAbilityUsed() {
            if(ability.HasTags(tags)) {
                charges--;
            }
            if(charges == 0) { Remove(); }
        }

        void Update() {
            
        }
    }

    ability.Get("Damage")

    Damage
    Healing
    Resource

    [Flat, FlatPoolPercentage, BonusPercentage]
    DamageDescriptor
        base: 100
        flatBonus: 200,
        percentage: 5,
        bonus: [2, 3, 5] (sort small to large)
        elements: ["Ice"]

    Properties: {
        "Elements": ["Ice"]
    }

    foreach modifier(ability)
        modifier.OnAbilityHit()

    foreach action
        action.OnDamageDealt()
        
    target.ApplyDamage(entity, ability, damageDescriptor)
    
    ability can implicitly do more damage to some things

    DamageDescriptor desc;
    ability.attributeModifiers.damage.each.Invoke(entity, ability, ref damageDescriptor)
    entity.ApplyDamage(damageDescriptor)

Status.Add("WhirlWind")
    Status.Apply() {
       var mod = Entity.AddAbilityModifier("id", [Tags], new Whirlwind(4));    
    }   

    Combat
    NonCombat

    DisableDoubleDamage(abilty) {
        ability.properties.doubleDamageVsFrozen = false
    }

*/
[SelectionBase]
public class AIEntity : Entity {
    public TextAsset jsonFile;
    public Entity target;

    protected NavMeshAgent agent;
    private Timer influenceUpdateTimer;
    private InfluenceMapSection iMapSection;
    public AIActionEvaluator evaluator;
    private Animator animator;

    //public void Awake() {
    //    //var asset = Resources.Load("AI/" + aiProfile) as TextAsset;
    //    //if(asset != null) {
    //    //    evaluator = new AIActionEvaluator(this, AIActionFactory.Create(this, asset.text));
    //    //}
    //}

    public override void Start() {
        base.Start();
        animator = GetComponent<Animator>();
        influenceUpdateTimer = new Timer(0.25f);
        agent = GetComponent<NavMeshAgent>();
        iMapSection = new InfluenceMapSection(9 * 9);
        AIAction[] actionPackage = null;
        if (jsonFile != null) {
            actionPackage = AIActionFactory.Create(this, jsonFile.text);
        }
        evaluator = new AIActionEvaluator(this, actionPackage);
    }

    public override void Update() {
        base.Update();
        evaluator.Update();
        iMapSection = InfluenceMapManager.Instance.UpdatePhysicalInfluence(transform.position, iMapSection);
        if (agent != null) {
            agent.speed = movementSpeed.Current;
        }
    }

    public void RefreshActionJson() {
        evaluator.Unload();
        AIAction[] actionPackage = AIActionFactory.Create(this, jsonFile.text);
        evaluator.AddActionPackage(actionPackage);
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(AIEntity))]
public class AIEntityInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if(GUILayout.Button("Refresh Action JSON")) {
            AIEntity agent = target as AIEntity;
            agent.RefreshActionJson();
        }
        if(Application.isPlaying && GUILayout.Button("Write Diagnostics")) {
            AIEntity agent = target as AIEntity;
            AIDecisionLog diagnostics = agent.evaluator.decisionLog;
            diagnostics.WriteToDisk(agent.name + "_AIDiagnostics.json");
        }
    }
}
#endif