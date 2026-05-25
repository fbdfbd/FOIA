using OneMoreSpoon.App.State;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.View.UI;
using VContainer.Unity;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.App.Tutorial
{
    public sealed class TutorialController : IStartable, ITickable
    {
        GameWorld world;
        SelectionState selectionState;
        TutorialDialogView dialogView;
        TutorialGoalView goalView;

        TutorialStep step;

        public TutorialController(GameWorld world, SelectionState selectionState, TutorialDialogView dialogView, TutorialGoalView goalView)
        {
            this.world = world;
            this.selectionState = selectionState;
            this.dialogView = dialogView;
            this.goalView = goalView;
        }

        public void Start()
        {
            // FixTutorialNodes();
            SetStep(TutorialStep.OpenGoal);
        }

        public void Tick()
        {
            switch (step)
            {
                case TutorialStep.OpenGoal:
                    if (goalView.WasOpened)
                        SetStep(TutorialStep.ConnectFryRoute);
                    break;

                case TutorialStep.ConnectFryRoute:
                    if (HasFryRoute())
                        SetStep(TutorialStep.MakeSoggyOnionFry);
                    break;

                case TutorialStep.MakeSoggyOnionFry:
                    if (HasStack("dish_o47") && HasStack("trait_sogginess"))
                    {
                        SetStep(TutorialStep.InspectSogginess);
                    }
                    break;

                case TutorialStep.InspectSogginess:
                    if (HasSelectedSubstance("trait_sogginess"))
                        SetStep(TutorialStep.DeleteFryInputEdge);
                    break;

                case TutorialStep.DeleteFryInputEdge:
                    if (!HasEdge("node_input", "node_fry"))
                        SetStep(TutorialStep.ConnectBakeRoute);
                    break;

                case TutorialStep.ConnectBakeRoute:
                    if (HasBakeRoute())
                        SetStep(TutorialStep.MakeUnderbrownedOnion);
                    break;

                case TutorialStep.MakeUnderbrownedOnion:
                    if (HasStack("dish_o49") && HasStack("trait_browning_lack"))
                    {
                        SetStep(TutorialStep.InspectBrowningLack);
                    }
                    break;

                case TutorialStep.InspectBrowningLack:
                    if (HasSelectedSubstance("trait_browning_lack"))
                        SetStep(TutorialStep.MergeCrisping);
                    break;

                case TutorialStep.MergeCrisping:
                    if (HasStack("edge_crisping"))
                        SetStep(TutorialStep.DeleteCutInputEdge);
                    break;

                case TutorialStep.DeleteCutInputEdge:
                    if (!HasEdge("node_input", "node_cut"))
                        SetStep(TutorialStep.ReconnectFryRoute);
                    break;

                case TutorialStep.ReconnectFryRoute:
                    if (HasFryRoute())
                        SetStep(TutorialStep.EquipCrisping);
                    break;

                case TutorialStep.EquipCrisping:
                    if (HasEquippedBlock("node_fry", "node_output", "edge_crisping"))
                        SetStep(TutorialStep.MakeCrispyOnionFry);
                    break;

                case TutorialStep.MakeCrispyOnionFry:
                    if (HasStack("dish_o48"))
                    {
                        SetStep(TutorialStep.Completed);
                    }
                    break;
            }
        }

        void SetStep(TutorialStep nextStep)
        {
            step = nextStep;

            switch (step)
            {
                case TutorialStep.OpenGoal:
                    dialogView.ShowMessage("목표 패널을 열어 오늘 만들 음식을 확인하고 바깥쪽을 눌러 창을 닫아보세요.");
                    break;

                case TutorialStep.ConnectFryRoute:
                    dialogView.ShowMessage("입력 -> 튀기기 -> 출력 순서로 노드를 연결하세요.\n시작 노드 우클릭 후 목표 노드를 좌클릭합니다.");
                    break;

                case TutorialStep.MakeSoggyOnionFry:
                    dialogView.ShowMessage("양파를 입력 노드에 올려 튀겨 볼까요?");
                    break;

                case TutorialStep.InspectSogginess:
                    dialogView.ShowMessage("눅눅한 양파튀김이 만들어졌습니다! 부산물인 눅눅함을 클릭해 합성 힌트를 확인해보세요.");
                    break;

                case TutorialStep.DeleteFryInputEdge:
                    dialogView.ShowMessage("바삭하게 만들 단서가 더 필요합니다.\n입력과 튀기기 사이의 연결을 선택하고 [Delete] 버튼을 눌러 삭제하세요.");
                    break;

                case TutorialStep.ConnectBakeRoute:
                    dialogView.ShowMessage("입력 -> 자르기 -> 굽기 -> 출력 순서로 새 공정을 연결해보세요.");
                    break;

                case TutorialStep.MakeUnderbrownedOnion:
                    dialogView.ShowMessage("양파를 입력 노드에 올려 새로운 결과를 확인하세요.");
                    break;

                case TutorialStep.InspectBrowningLack:
                    dialogView.ShowMessage("덜 볶인 양파가 만들어졌습니다!\n부산물인 갈변부족을 선택해 합성 힌트를 확인해보세요.");
                    break;

                case TutorialStep.MergeCrisping:
                    dialogView.ShowMessage("눅눅함과 갈변부족을 합성 노드에 올려 바삭화 블럭을 만드세요.");
                    break;

                case TutorialStep.DeleteCutInputEdge:
                    dialogView.ShowMessage("이제 개선된 튀김을 만들어볼까요?\n입력과 자르기 사이의 연결을 삭제하세요.");
                    break;

                case TutorialStep.ReconnectFryRoute:
                    dialogView.ShowMessage("입력 노드를 튀기기 노드에 다시 연결하세요.");
                    break;

                case TutorialStep.EquipCrisping:
                    dialogView.ShowMessage("바삭화 블럭을 튀기기와 출력 사이의 연결 위에 올려놓으세요.");
                    break;

                case TutorialStep.MakeCrispyOnionFry:
                    dialogView.ShowMessage("양파를 다시 넣어 바삭한 양파튀김을 완성해볼까요?");
                    break;

                case TutorialStep.Completed:
                    dialogView.ShowComplete("완성했습니다! 더 많은 공정을 통해 최종 요리를 완성해보세요!");
                    break;
            }
        }

        void FixTutorialNodes()
        {
            foreach (var pair in world.Nodes)
            {
                string definitionId = pair.Value.DefinitionId;

                if (definitionId != "node_input" &&
                    definitionId != "node_output" &&
                    definitionId != "node_merge" &&
                    definitionId != "node_cut" &&
                    definitionId != "node_bake" &&
                    definitionId != "node_fry")
                {
                    continue;
                }

                if (world.Tags.TryGetValue(pair.Key, out var tags))
                    tags.Add("Fixed");
            }
        }

        bool HasFryRoute()
        {
            return HasEdge("node_input", "node_fry") &&
                   HasEdge("node_fry", "node_output");
        }

        bool HasBakeRoute()
        {
            return HasEdge("node_input", "node_cut") &&
                   HasEdge("node_cut", "node_bake") &&
                   HasEdge("node_bake", "node_output");
        }

        bool HasStack(string substanceId)
        {
            foreach (var stack in world.SubstanceStacks.Values)
            {
                if (stack.SubstanceId == substanceId)
                    return true;
            }

            return false;
        }

        bool HasSelectedSubstance(string substanceId)
        {
            if (selectionState.SelectedType != SelectionTargetType.Substance)
                return false;

            if (!world.SubstanceStacks.TryGetValue(selectionState.SelectedEntityId, out var stack))
                return false;

            return stack.SubstanceId == substanceId;
        }

        bool HasEdge(string fromDefinitionId, string toDefinitionId)
        {
            foreach (var edge in world.Edges.Values)
            {
                if (!world.Nodes.TryGetValue(edge.FromNodeId, out var fromNode))
                    continue;

                if (!world.Nodes.TryGetValue(edge.ToNodeId, out var toNode))
                    continue;

                if (fromNode.DefinitionId == fromDefinitionId &&
                    toNode.DefinitionId == toDefinitionId)
                {
                    return true;
                }
            }

            return false;
        }

        bool HasEquippedBlock(
            string fromDefinitionId,
            string toDefinitionId,
            string blockId)
        {
            foreach (var pair in world.Edges)
            {
                var edgeId = pair.Key;
                var edge = pair.Value;

                if (!world.Nodes.TryGetValue(edge.FromNodeId, out var fromNode))
                    continue;

                if (!world.Nodes.TryGetValue(edge.ToNodeId, out var toNode))
                    continue;

                if (fromNode.DefinitionId != fromDefinitionId ||
                    toNode.DefinitionId != toDefinitionId)
                {
                    continue;
                }

                if (!world.EdgeBlockSlots.TryGetValue(edgeId, out var slot))
                    return false;

                return slot.Contains(blockId);
            }

            return false;
        }
    }
}