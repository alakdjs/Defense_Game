# Unity_3D_Project (ReadMe 수정중)
# Santa Survival (Santa-Survivor-Defense_Game)

Santa Survival은 Unity로 개발한 3D Wave Defense Survival Game입니다.
플레이어는 산타 캐릭터를 조작하여 몰려오는 몬스터 웨이브를 막아내고, 다양한 증강 시스템과 무기 속성 시스템을 활용해 생존해야 합니다.

게임은 자동 공격 기반 전투 시스템과 로그라이크 형태의 웨이브 디펜스 구조를 결합하여 진행되며,
플레이어는 전투 중 획득하는 증강을 통해 캐릭터의 능력을 강화하고 다양한 전투 스타일을 만들어낼 수 있습니다.

또한 몬스터, 펫, 플레이어 모두 FSM 기반 AI 구조로 설계되어 있으며
무기 및 증강 시스템은 ScriptableObject 기반 데이터 구조로 구현되었습니다.
<img width="400" height="225" alt="1_게임시작화면" src="https://github.com/user-attachments/assets/7d3abc07-a2b7-4796-a777-79a874dbe45d" />

<img width="400" height="225" alt="2_컷씬장면" src="https://github.com/user-attachments/assets/e7a309ba-4499-46fe-ae4e-ee12a8775088" />

<img width="400" height="225" alt="4_증강선택" src="https://github.com/user-attachments/assets/bd532072-232e-4590-bd77-9bc89acdf50e" />

<img width="400" height="225" alt="5_게임진행중" src="https://github.com/user-attachments/assets/4671a0fd-607e-402a-864c-c57d7eea1fa1" />

<img width="400" height="225" alt="6_보스몬스터조우" src="https://github.com/user-attachments/assets/a31c04de-0d79-415e-a9e8-8db5da75d130" />

<img width="400" height="225" alt="7_최종보스" src="https://github.com/user-attachments/assets/7349bc39-1135-4681-8871-e6b933a993a0" />

<br>

## 🛠 개발 환경

- Engine: Unity 6000.0.54f1 LTS
- Language: C#
- IDE: Microsoft Visual Studio 2022
- Version Control: Git
- Window 11

<br>

## 🗓 개발 기간

**2025.12 ~ 2026.02 (약 2개월)**

<br>

## 사용 기술

Architecture
- FSM (Finite State Machine)
- ScriptableObject Data Architecture
- Component-based Design

Core Systems
- Player FSM System
- Monster AI System
- Pet AI System
- Wave System
- Augment System
- Weapon System
- Elemental Damage System
- Tower Defense System

UI Systems
- Player HP UI
- Monster, Pet HP Bar
- EXP / Level UI
- Wave Progress UI
- Attack Range UI

Additional Systems
- Camera Occlusion System
- Async Scene Loading
- Game State Manager
- Cutscene System (ScriptableObject 기반)

<br>

## 구현 기능 (시스템)

Player System
플레이어는 FSM 기반 상태 시스템으로 동작합니다.

상태는 다음과 같이 구성됩니다.
- Idle
- Move
- Dead

플레이어 입력은 두 가지 방식으로 구현되었습니다.

Movement System
플레이어 이동은 다음 두 가지 입력 방식을 동시에 지원합니다.

Mouse Movement
- 마우스 클릭 위치로 이동
- Raycast 기반 이동 위치 탐색

Keyboard Movement
- WASD 이동 지원
- 키보드 입력 발생 시 기존 마우스 이동 목표 자동 취소

Auto Attack System
플레이어는 일정 시간마다 자동 공격을 수행합니다.

공격은 공격 쿨타임 기반 타이머 시스템으로 구현되어 있으며
무기 타입에 따라 서로 다른 공격 방식이 실행됩니다.

Sword
- 근접 공격
- OverlapSphere 기반 공격 판정
- 애니메이션 이벤트 기반 공격 처리

Rifle
- 원거리 공격
- Projectile 기반 총알 발사 시스템

Auto Targeting System
플레이어는 일정 범위 내에서 가장 가까운 몬스터를 자동으로 탐지하여 조준합니다.

탐지는 Physics.OverlapSphere 기반으로 구현되어 있으며
회전은 Quaternion.Slerp를 사용하여 자연스럽게 처리됩니다.

Weapon System
무기 시스템은 ScriptableObject 기반 데이터 구조로 구현되었습니다.

각 무기는 다음 정보를 포함합니다.
- Weapon Type
- Attack Range
- Damage
- Element Type
- Weapon Prefab

무기 장착 시
- 무기 Prefab 생성
- 공격 범위 동기화
- 플레이어 속성(Element) 적용
- 공격 범위 UI 갱신

Elemental Combat System
게임에는 속성 상성 시스템이 존재합니다.

속성 종류
- Normal
- Fire
- Water
- Electric
- Ice
- Rock
무기와 몬스터는 각각 속성을 가지며
속성 상성에 따라 데미지 배율이 적용됩니다.

Augment System
전투 중 플레이어는 증강 카드를 선택하여 캐릭터 능력을 강화할 수 있습니다.

증강 시스템은 ScriptableObject 기반으로 설계되었으며
다양한 능력치 증가 및 특수 효과를 제공합니다.

증강 예시
- 공격력 등의 능력치 증가
- 플레이어, 타워, 펫의 능력치 증가
- 공격 속도 증가
- 공격 사거리 범위 증가
- 체력 회복
- 펫 소환
- 몬스터 기절
- 경험치 흡수 범위 증가
- 파동탄 (패시브 추가)
- 무기 속성 변경

Monster AI System
몬스터는 FSM 기반 AI 구조로 구현되었습니다.

몬스터 상태
- Idle
- Chase
- Attack
- Dead
- Stun

몬스터는 NavMesh 기반 이동 시스템을 사용하며
플레이어 또는 타워를 목표로 이동하여 공격합니다.

Pet System
펫은 플레이어를 돕는 서브 타워 형태의 AI 유닛입니다.

펫 역시 FSM 구조로 동작합니다.

펫 상태
- Idle
- Chase
- Attack
- Dead

펫은 일정 범위 내 몬스터를 탐지하여 자동으로 공격하며
공격 후 Idle 상태로 돌아왔을 때 범위를 벗어났을 경우 스폰 위치로 복귀하도록 구현되었습니다.

Wave System
게임은 웨이브 기반 진행 시스템으로 구성됩니다.

각 웨이브마다
- 몬스터 스폰
- 보스 등장
- 웨이브 진행 UI 표시
또한 전체 게임 진행도를 표시하는 TimeBar UI가 구현되어 있습니다.

Tower Defense System
맵 중앙에는 타워 오브젝트가 존재하며
몬스터는 플레이어와 함께 타워를 공격 대상으로 삼습니다.

타워 체력이 0이 되면 게임이 종료됩니다.

Level & EXP System
몬스터 처치 시 경험치 Sphere가 드롭되며
플레이어는 일정 범위 내에서 경험치를 자동으로 흡수합니다.

경험치가 일정량에 도달하면 플레이어 레벨이 상승하며
레벨업 시 플레이어 능력치가 증가합니다.

Camera System
카메라에는 Occlusion 처리 시스템이 적용되었습니다.

플레이어와 카메라 사이에 큰 오브젝트가 위치할 경우
Raycast 기반으로 해당 오브젝트를 반투명 처리하여 시야를 확보합니다.

Cutscene System
게임 시작 및 클리어 연출을 위해 ScriptableObject 기반 컷씬 시스템을 구현했습니다.

컷씬 데이터는 ScriptableObject로 관리되며
씬 전환 및 UI와 연동되어 스토리 연출을 수행합니다.

<br>

## 기술 문서 (기술서)

프로젝트의 상세 기술 구현 내용은 아래 문서에서 확인할 수 있습니다.

<br>

## 플레이 영상

링크 추가 예정

<br>

## 게임 다운로드

PC:
모바일:
