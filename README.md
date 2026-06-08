# [Vertical Scrolling Shooter]
코드 상세 설명<br/>
[Player](./Player_Code_Details.md)<br/>
[Item](./Item_Code_Details.md)<br/>
[Enemy](./Enemy_Code_details.md)<br/>
[GameManager](./GameManager_Code_details.md)<br/>
[UI](./UI_Code_details.md)<br/>
[README](./README.md)<br/>
[Background](./Background_Code_details.md)<br/>

라이센스<br/>
[LICENCE](./LICENCE.md)<br/>

유니티 6(Unity 6) 엔진을 기반으로 개발된 크로스 플랫폼 2D 아케이드 게임 프로젝트입니다.  
다양한 오브젝트 및 에셋 병합을 통해 완성도를 높였습니다.

---

## 1. 프로젝트 개요 및 용도
**프로젝트 목적:** 유니티 6 엔진의 2D 기능과 효율적인 깃(Git) 브랜치 병합 프로세스를 이해하고 협업에 적용하기 위한 과제 프로젝트입니다.<br/> <br/>
**주요 기능:** 
- **2D 게임플레이 구조:** 플레이어 제어, 투사체 발사, 적(Enemy) 생성 및 스폰 시스템 구축

- **UI 및 스코어 시스템:** TextMeshPro(TMP), ScriptableObject(SO)를 활용한 실시간 점수(Score) 반영 및 최고 기록(High Score) 저장 기능, 게임 오버 뷰(GameOverView) 구현

- **Windows, Linux 플랫폼 지원:** 단일 C# 소스 코드를 바탕으로 Windows, Linux 환경으로 빌드 및 실행이 가능

---

## 2. 게임 사용법 및 조작 지침 (Controls)
본 게임은 유니티의 **Input System**을 사용하여 키보드 및 마우스 입력을 처리합니다.

- **이동 (Move):** `W`, `A`, `S`, `D`
- **공격/궁극기 (Fire/Ult):** `K` 키를 통해 공격, `L` 키를 통해 궁극기 사용
- **UI 조작:** 메인 화면에서 `Start`, `Quit` 버튼 상호작용 가능. 게임 오버 화면, 클리어 화면, `ESC` 키 입력을 통한 정지 메뉴에서 마우스 클릭을 통해 `Restart(재시작)`, `Main Menu(메인화면)`, `Quit(종료)` 버튼 상호작용 가능
- **아이템**: 플레이어 총알 데미지 증가, 체력 회복, 궁극기 사용 가능 횟수 추가, 보조 무기 추가, 점수 추가 총 5 종의 아이템이 존재하며, 적 오브젝트 파괴 시 일정 확률(30%)로 아이템 생성

---

## 3. 빌드 및 실행 방법 (Compilation & Execution)

### 개발 환경 및 요구 사항
**Engine Version:** Unity 6 (6000.3 이상 권장) <br/>
**Packages:** TextMeshPro(TMP), Unity UI(uGUI), Input System <br/>
**Target Platforms:** Windows (`.exe`), Linux(`Executable`) <br/>

---

### 소스 코드 컴파일 및 에디터 실행 방법
프로젝트 소스 코드를 유니티 에디터에서 열고 컴파일하는 방법입니다.

1. **저장소 클론:** 본 깃허브 저장소를 로컬 컴퓨터로 클론(Clone)하거나 release 탭에서 ZIP 파일을 다운로드하여 압축을 해제합니다.
2. **프로젝트 로드:** `Unity Hub`를 실행한 뒤 **[추가] -> [디스크에서 프로젝트 추가]** 를 눌러 본 프로젝트 폴더를 선택합니다.
3. **패키지 동기화:** 유니티 6 에디터가 처음 켜질 때 `manifest.json`에 정의된 `Unity UI`, `TextMeshPro`, `Input System` 패키지를 자동으로 다운로드하고 컴파일합니다.
   *주의:* 만약 에디터 실행 시 인풋 시스템 백엔드 활성화 팝업이 뜨면 **[Yes]** 를 눌러 에디터를 재부팅해 주세요.
4. **게임 실행:** 에디터 상단의 || (Play) 버튼을 누르면 즉시 게임 코드가 컴파일되며 에디터 내에서 플레이가 가능합니다.

---

### 플랫폼별 최종 빌드(Build) 및 실행 방법
프로젝트 컴파일 완료 후 각 운영체제별 실행 파일을 추출하고 구동하는 방법입니다. <br/>
현재 release 탭에서 Windows, Linux 환경을 지원하는 실행 파일이 존재하며, 각 운영체제에 맞는 압축 파일을 다운받아 압축 해제하고, 실행 파일을 통해 프로젝트 실행 가능합니다. *주의: 압축 해제한 파일에서 실행 파일을 제외한 파일 또한 게임 실행에 필요하기 때문에 훼손하면 안됩니다.* <br/>
아래는 저장소 클론 및 zip 파일을 다운받아 유니티 환경에서 빌드하는 방법입니다.
유니티에서 Windows, macOS, Linux 운영체제를 지원하지만, 최종 빌드 시 Windosw를 제외하면 추가 모듈이 필요하며 macOS에서 실행 가능한지 확인할 수 없어 Windows, Linux 실행 파일만 release 탭에서 다운로드 받을 수 있습니다.

#### Windows (PC)
1. 유니티 상단 메뉴 **[File] -> [Build Settings]** 로 이동합니다.
2. 플랫폼을 **[Windows]** 로 지정하고 **[Build]** 를 누른 뒤, 비어있는 새 폴더(예: `Build_Folder`)를 생성해 경로로 지정합니다.
3. 빌드가 완료되면 생성된 `.exe` 실행 파일을 더블 클릭하여 실행합니다.

#### macOS
1. **[Build Settings]** 에서 플랫폼을 **[Mac 전용 독립형]** 으로 전환(Switch Platform)합니다. *(유니티 허브에서 Mac Build Support 모듈 설치 필요)*
2. **[Build]** 를 눌러 파일(예: `opensourceSW.app`)을 추출합니다.
3. *보안 경고 해결:* 타 운영체제에서 빌드된 파일 특성상 Mac에서 실행 시 '확인되지 않은 개발자' 경고가 뜰 수 있습니다. 이 경우 Mac의 `[시스템 설정] -> [개인정보 보호 및 보안]`에서 **[확인 없이 열기]** 를 누르거나 터미널에서 보안 설정을 해제한 후 실행해 주세요.

#### Linux
1. **[Build Settings]** 에서 플랫폼을 **[Dedicated Server]** 또는 **[Linux 독립형]** 으로 전환합니다.
2. **[Build]** 를 눌러 리눅스 바이너리 실행 파일을 추출한 뒤, 해당 환경 터미널에서 실행 권한(`chmod +x`)을 부여하고 구동합니다.

---

## 4. 저장소 관리 및 병합 이력 (Git Flow)
본 프로젝트는 Git Flow 방식을 채택하여 main, develop, release, feat 브랜치를 주력으로 협업했고, main, develop 브랜치는 브랜치 보호 규칙을 세워 PR을 통한 병합만 허용했습니다.<br/>
기본적으로 feat 브랜치에서 develop 브랜치로 PR을 통해 병합을 하고 develop 브랜치에서 release 브랜치를 생성, main 브랜치에 PR을 통한 병합을 하지만, 코드 및 패키지 충돌을 해결하기 위해 `mergefix` 브랜치를 활용하여 수동 물리 검증을 거쳤습니다.