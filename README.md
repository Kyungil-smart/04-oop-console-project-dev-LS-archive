[![Open in Visual Studio Code](https://classroom.github.com/assets/open-in-vscode-2e0aaae1b6195c2367325f4f02e2d04e9abb55f0b24a779b69b11b9e10269abc.svg)](https://classroom.github.com/online_ide?assignment_repo_id=22157903&assignment_repo_type=AssignmentRepo)

![라이더 자동생성 다이어그램](<Type Dependencies Diagram for Program and other elements.png>)

최종 작업 전에는 메모장으로 사용 예정

- 기본적 맵 이동 및 적 조우 및 전투 구현 시도.
- 적 조우시 전투씬으로 이동 및 전투.
- 전투 종료시 기존 맵으로 다시 전환

게임 콘텐츠보다는 렌더링 기법 및 쾌적한 유저 경험에 집중한 프로젝트

## 조작법 정리
- 화살표: 이동
- Enter: 결정
- I: 인벤토라
- L: 디버그
- T: 임시 체력 삭제 버튼

## 핵심 렌더링 로직
Init (초기화) -> Update (입력 대기) -> Render (그리기)
- 순서 변경 (Input → Update → Render)
    - 기존: 그리기 → 입력 → 처리
    - 변경: 입력 → 처리 → 그리기

## 컨텐츠 요소
- 타이틀 씬: 게임 시작, 크레딧창, 게임 종료
- TownScene: 맵 탐색 및 이동, 포션(HP, MP), 함정, 적 조우시 전투씬 연결
- BattleScene: 적 조우시 진행하는 전투씬. 전투 끝나면 TownScene으로 복귀 

## 작업 노트

타이틀 제목 꾸미기.  
메뉴 꾸미기  

- TextExtensions의 출력 기능에 널러블(Nullable) 허용해 컬러값을 넣는 경우와 안넣는 경우 다 허용 가능하게 해서 사용 편의성 높임.

- AsciiArtData 클래스 제작
  - AsciiArt 구조체 만들어서 
  - 데이터 정리 및 아스키 
  - 데이터의 정보 값 접근 가능

- TextExtensions에 아스키 Draw 메서드 추가

- TitleScene의 Render 개선 및 커스텀
  - 게임 이름 외각선 설정
  - 로고가 올 위치 중앙에 타이틀 위치 설정
  - 게임 이름 외곽선 그리기
  - 게임 이름 출력(외곽선 안쪽)
  - 로고 외곽선 출력
  - 아스키 데이터에서 로고 데이터 불러와서 그리기
  - 메뉴 출력(로고 아래쪽 중앙 정렬)
    - _logoOutline.X + (_logoOutline.Width / 2) - (_titleOutline.Width / 2)

- Ractangle에 외부 호출시 요소 설정의 번거로움 줄이기 위한 오버라이드 추가.
  - 사용을 시도해보았지만 위치정렬을 위한 계산 과정에서 서로의 값을 참조할 경우 불편이 생겨 추후 필요한 경우에 사용하기로하고 안씀 

- 크레딧창 구현
  - Console.ReadKey(true)를 통해 메인 루프에 들어가 크레딧 창이 지워지는 것을 방지

- 키보드 연속 입력시 화면 렌더링 지연을 방지하기 위해 입력 딜레이 구현 추가(InputManager)
  - Thread.Sleep 이용
  - 시도해보니 GameManager Run 루프에서 Clear 호출이 문제됨.
- 해결 방법 접근
  - Console.Clear() 제거:
    - 각 씬(Scene)의 Render 메서드 안에서 화면을 지워야 할 때만 Clear()를 호출하도록 책임을 넘김.
  - 순서 변경 (Input → Update → Render)
    - 기존: 그리기 → 입력 → 처리
    - 변경: 입력 → 처리 → 그리기
- 구현 방식
  - 화면 전체를 지우지 말고, 변하는 숫자나 텍스트만 커서를 이동해서 그 위에 덮어쓰는 방식으로 구현
  - 씬 이 변했는지 체크하고 변했으면 다시 그려줌
  - 메인 루프에서 클리어 대신 일단 필수적으로 씬에 들어갈때 한번 클리어.
    ```cs
    public virtual void Enter()
    {
        Console.Clear();
    }
    ```
  - Enter 메서드 추상 메서드에서 가상 메서드로 변경
    - 오버라이딩 Enter에 base.Enter(); 한줄씩 추가.
  - 1초에 30fps로 제한 WAIT_TICK = 1000 / 30
- 무한 입력은 입력 딜레이 방식으로 해결되었지만 30 fps라도 플리커링 현상은 여전함
  - 해결 방안: 입력이 들어올때만 Render를 실행할 수 있게 개선. (**"Dirty Flag 패턴"**)
    - 키입력을 씬 클래스 차원에서 체크하고 키입력이 있을때 렌더링 실행하기.
    ```cs
    // InputManager에서 키입력이 있었는지 체크용 변수
    public static bool HasInput => Console.KeyAvailable;
    // 다시 그려줄 필요가 있는지 체크용 변수
    protected bool NeedsRedraw { get; set; } = true;
    // Update에서 키 입력시 true 그리기 끝나면 false
    ```
- 결과는 성공적이지만 키를 꾹 눌렀을때 입력이 처리보다 빨라 밀리는 현상 발생
  - 해결 방안: 키입력을 체크하는 GetUserInput에서 딜레이안에 들어온 키는 버리고 코드의 마지막 단계에서 입력 한번에 쓰인 값을 제외하고 뒤에 쌓인 키들 버려서 중복 입력을 원천 차단한다.
    ```cs
    // 마지막 입력 후 InputDelay(0.1초)가 안 지났으면 입력 무시
    if ((DateTime.Now - _lastInputTime).TotalMilliseconds < InputDelay)
    {
        // 입력 버퍼 비우기 (중복 입력 방지)
        while (Console.KeyAvailable) Console.ReadKey(true); 
        return;
    }
    // 중복 입력 원천 차단. 입력 한번에 쓰인 값을 제외하고 뒤에 쌓인 키들 버리기.
    while (Console.KeyAvailable)
    {
        Console.ReadKey(true);
    }
    ```
    이렇게 하면 입력이 끝낱을때 바로 동작이 멈추는걸 확인 가능했다.

- 크레딧창 수정
  - 바뀐 렌더링 방식에 따라 **"Dirty Flag 패턴"** 적용
  - 기존 Console.ReadKey() 제거
  - Input -> Update -> Render 흐름을 그대로 따름
  - 기존의 ViewCredits()를 RenderCredits와 분리하고 CloseCredits 기능을 추가하며 상태 체크용 변수 값 변경으로 입력 값에 따라 자연스레 업데이트 되도록 변경
  - RenderTitle, RenderCredits 두 메서드로 분리해 상태에 따라 필요한 화면 렌더링
  - CloseCredits 할 때 해당 영역을 공백으로 덮어쓰기.
  - Init()에서 크레딧창 정보 미리 설정
    - _creditsOutline 구조체 변수 활용
  - 타이틀 정보도 Init()에서 미리 정의하기
    - 유지보수 개선 효과 획득
  
  - 추후 시간 여유날시 렌더링 개선. Menu리스트만 덮어쓰기 방식으로

- TownScene 수정.
  - 타이틀씬과 동일한 방식의 렌더링 적용하도록 코드 수정작업
  - 맵 위쪽 잔상 제거용 메서드 ClearUpperArea() 
    - 게이지가 그려질 수 있는 _startY 위쪽 2줄을 공백으로 덮어씀
    - PrintField()에서 왼쪽 오른쪽 맵 밖을 공백으로 출력해주기
- LogScene 수정: **"Dirty Flag 패턴"** 적용
  - 적용 안하면 바뀐 렌더링 방식에 최적화된 코드가 아니라 무한 반복해서 수정.
- TownScene에도 타이틀 이름 출력 추가 시도
  - 깔끔하지 않음
- UI 구조 변경
  - 왼쪽 맵, 오른쪽 정보창으로 변경
  - 레이아웃 재구성 작업 진행
  - ClearUpperArea() 필요 없어짐(주석 처리)
  - 체력과 마나도 맵이 아니라 옆에 정보창에 출력
  - 맵을 사각형으로 감싸주는 과정에서 양옆 지워짐.
    - 기존 게이지가 UI창으로 옮겨질테니 기존 코드 주석처리
  - 상단에 맵 이름 가이드 출력
  - 플레이어에서 정보를 얻어와 우측 정보창에 출력.

- 완성 후 살펴보니 결과는 괜찮지만 드로잉에 시간이 걸려 렉처럼 느껴짐,
- 성능 개선
  - StringBuilder 도입하기
  - Console.Write의 잦은 호출이 문제.
  - StringBuilder를 사용하여 한 줄을 완성한 뒤, Console.Write를 한 번만 호출하도록 TownScene.cs의 PrintField를 수정
  - 배칭(Batching)과 유사한 최적화 형태
  - 성능 개선은 되었지만 여전히 이동이 느린 느낌
  - 개선 시도
    - 배경(맵, UI 틀)은 Enter에서 딱 한 번만 그리기
    - 이동할 때는 이전 위치의 플레이어를 지우고(바닥 복구), 새 위치에 플레이어를 그리는 2번의 Write만 수행.
    - Tile 구조체 기능 추가
      - GetRenderSymbol: 오브젝트가 있으면 심볼값, 없으면 기본값인 바닥 심볼 출력
      - SetObject: 타일 설정
      -  ClearObject: 타일 지우기, 타일이 없으면 바닥 심볼이 된다.
      -  PlayerCharacter에서 이동시 이전 위치 저장해주고 Town 씬의 Render에서 플레이어 위치 바뀌었을시 이전 위치를 지워주고 맵 데이터에서 이전 위치의 데이터를 얻어와 출력한다
   -  개선 성공

-  하지만 인벤토리가 깔끔히 클리어 되고 있지 않음
   -  이유: 플레이어가 그리는 기존 방식에서 개선 되지 않았음
   -  해결 방안 게임 씬 중앙에 보기 좋은 위치에 띄우기.
   -  타이틀씬의 크레딧 같은 방식이지만 타운씬에서 쓴 최적화 및 코드 양식 반영
      -  TownScene에서 맵 중앙 좌표를 계산하여 인벤토리로 전달
      -  TownScene에서 렌더시 인벤토리가 닫혔다면 전체 맵을 다시 그려서 잔상 제거
      -  인벤토리가 열려 있는 상태라면 맵 위에 인벤토리를 덮어씌움
      -  둘 다 아니고 일반 이동 상태에선 기존처럼 부분적으로 덮어쓰기 실행.
      -  인벤토리 아이템을 사용하거나 닫을때도 해당 지역만 다시 그려줌
      -  인벤토리에서 가로 세로 값을 받아와 인벤토리 부분 갱신 메서드 추가
      -  RestoreInventoryBackground()
      -  해당 기능 추가에 따라 인벤토리에서 임의로 추가한 가이드
      -  메세지도 동적으로 위치 조정
      -  이전 프레임과 현재 프레임을 비교하는 방식으로 잔상 제거
   -  문제 해결, 하지만 인벤토리를 끌시 맵은 복구되지만 외곽선이 복구되지 않음
      -  TownScene의 RestoreInventoryBackground 메서드를 보면, 인벤토리가 있던 자리를 복구할 때 맵 데이터 범위 밖인 경우 무조건 공백(' ')을 출력하던게 문제
      -  인벤토리 잔상을 지운 후에 맵 외곽선을 다시 한번 그려주기
      -  기존 아웃라인 드로우 코드는 맵을 지워버리기에 다시 수정할 시간을 들이기 보다는 일단 사용하지 않고 맵을 전체 출력하는 PrintField 사용
      -  인벤토리가 정보창(오른쪽 UI)까지 침범해서 인벤토리 테두리도 다시 그림, _infoOutline.Draw()
      -  인벤토리 배경에 색을 넣으니 맵과 정보창 사이가 안지워짐.
         -  RestoreInventoryBackground 공백 좌우 처리 영역 확장으로 해결(하드코딩)
      -  다시 문제 확인: 인벤토리 아이템 사용후 닫으면 잔상이 남음
         -  인벤토리 호출 당시 가장 큰 상태를 기억해서 지워주기
          ```cs
          // 인벤토리를 켠 상태에선 못움직이고 사용만 가능하니 출력 시점이 제일 Height가 큼
          currentMaxHeight = actualHeight + 4; // 여유 공간
          ```
- 드디어 컨텐츠 구현 시작
- 맵에 적과 함정 배치, 체력 포션과 마나 포션 배치(색깔로 구분, 인벤토리에서는 텍스트로)
  - 열거형으로 포션 타입 정의
  - 생성자에서 타입과 회복량을 받도록 설정
  - Init()에서 포션 이름과 컬러 설정
  - 기존의 스트링 빌더 형식 출력으로는 포션에 색을 입힐 수 없음
  - 필요에 따라 끊어서 출력하기
    - 오브젝트(포션 등)를 만났을 때
    - 지금까지 StringBuilder에 쌓인 일반 타일들을 먼저 출력
    - 포션처럼 색상 지정 필요할경우 생상 지정해서 출력
    - 그 외에 일반 오브젝트 같은 경우 기본색 출력
    - 오브젝트가 아닌 일반 타일(바닥)은 StringBuilder에 계속 쌓음
    - 마지막에 남은 StringBuilder 내용 출력
- 함정 구현(^)
  - 상호작용시 플레이어가 1데미지를 입고 사라짐
- 정보창에 [ Battle LOG ] 영역 추가
  - 필드에 배치된 오브젝트 설명
-  Battle LOG 텍스트들이  인벤토리를 열어도 보임 수정
   -  Render() 메서드 마지막에 UpdateUIStats()가 항상 호출되도록 되어 있기 때문.
   -  인벤토리가 열려 있을 때는 인벤토리 화면이 우선권을 가져야 하므로, 플레이어가 제어권을 가졌을 때(IsActiveControl)에만 UI를 갱신하도록 Render()를 수정.


## 확장 가능성
- 인벤토리창에서 아이템 설명 기능 추가
- 배틀 씬 전환시 데이터 유지
- 배틀 씬에서 아스키 아트를 통한 적 렌더링
  - 리소스를 구하거나 만들기 힘든 방식이 아닌 텍스트를 아스키 아트로 바꾸어 적을 그리는데 사용.
    - 적 HP에 따라 아스키 아트의 색상 변경
    - 타격 시 아스키 아트 색상 변경을 통한 타격 이펙트 연출 등
- 심화 가능성
  - 아스키 아트나 긴 텍스트같은 데이터를 외부 파일에서 불러와 사용 
  - 아스키 아트 리소스 순차 출력을 통한 애니메이션 혹은 배열 순서에 접근하는 애니메이션.