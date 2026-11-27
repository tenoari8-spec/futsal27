# FUTSAL 27

Simulador de futsal brasileiro 3D para Android (projeto pessoal e experimental, sem fins comerciais).
Feito em **Unity 2022.3 LTS** (C#), com foco em aparelhos de entrada (referência: Moto E7 Plus, Android 10).

## Status: Etapa 2 de 10 (quadra 3D, jogadores e bola)

Já funciona:
- Menu principal animado (lista de modos + painel de destaque), tudo montado por código
- Partida Rápida: seleção de times (casa x visitante) e de uniformes (principal/reserva), com aviso de cores parecidas
- Base de clubes editável em JSON, com os 12 clubes iniciais
- Escudos em PNG inseridos pelo usuário (ou escudo genérico com a sigla)
- Configurações: qualidade baixa/média/alta e duração do tempo; salvas localmente
- Orientação horizontal, botão "voltar" do Android
- Workflow do GitHub Actions para gerar o APK

Etapa 2 (partida 3D, **ainda não testada no Unity**):
- Quadra coberta 40 x 20 m com linhas oficiais, gols 3 x 2 m com rede, placas e arquibancadas
- Bola com física própria (quique ~0,55, rolamento, efeito Magnus) e detecção de gol com placar
- 10 jogadores provisórios (primitivas) em formação 1-2-1, goleiros de cor diferente
- Câmera esportiva com 3 ângulos (botão CÂMERA) e placar de TV
- Botões de teste (RASTEIRO / ALTO / REINICIAR) para validar a física da bola
- Os jogadores ainda não se movem nem são controlados (Etapa 3)

Ainda **não** existe: controle dos jogadores, IA, regras (próximas etapas).

## Como abrir
1. Instale o **Unity Hub** e o editor **2022.3 LTS** com o módulo **Android Build Support** (SDK, NDK, OpenJDK).
2. Abra esta pasta como projeto no Unity Hub (a primeira importação demora).
3. Menu **Futsal27 > 1 - Configurar Projeto** (cria a cena e configura o Android).
4. Aperte **Play**.
5. APK: **Futsal27 > 2 - Gerar APK Android** (saída em `build/Android/`).

## Clubes e escudos
No primeiro uso, o jogo cria `clubs.json` e a pasta `Escudos` em `Application.persistentDataPath`
(Android: `Android/data/com.futsal27.game/files/`). Edite o JSON (nome, cores, notas) e coloque os PNGs dos
escudos com o nome indicado em `crestFile` (ex.: `magnus.png`).
As **cores e notas iniciais são placeholders**: ajuste com os dados reais.

## Subir no GitHub
```bash
git init
git add .
git commit -m "Etapa 1: estrutura e menu principal"
git branch -M main
git remote add origin https://github.com/SEU_USUARIO/futsal27.git
git push -u origin main
```
Depois de abrir o projeto no Unity pela primeira vez, faça commit também dos arquivos `.meta` gerados.

## APK pelo GitHub Actions (sem PC de build)
O workflow `.github/workflows/build-android.yml` usa o `game-ci/unity-builder`. Ele precisa dos secrets
`UNITY_LICENSE`, `UNITY_EMAIL` e `UNITY_PASSWORD` (licença Unity Personal).
A ativação da licença é feita uma vez, seguindo https://game.ci/docs/github/activation. O APK sai em *Actions > Artifacts*.

## Arquitetura
```
Assets/Scripts/
  Core/   Bootstrap, ScreenManager, GameSession, GameSettings, QualityManager
  Data/   ClubData, ClubRepository (JSON), CrestLoader
  UI/     UIKit (helpers uGUI), SpriteFactory, telas do menu
  Match/  MatchRoot, CourtBuilder, BallController, PlayerAvatar, MatchCamera, MatchHud
Assets/Editor/  ProjectSetup, BuildScript
```
Próximos módulos: `Match` (regras e relógio), `Ball` (física própria), `Players` (controle, animação, IA), `Camera`, `Input`.

## Roadmap
1. Estrutura e menu ✔ · 2. Quadra 3D, jogadores e bola ✔ (a validar) · 3. Movimentação e controles touch ·
4. Passes, chutes e goleiros · 5. IA · 6. Regras e arbitragem · 7. Clubes e uniformes (editor visual) ·
8. Modos de jogo · 9. Gráficos, animações e som · 10. Otimização e APK final

Nenhum código, modelo, textura ou arquivo de terceiros foi usado; a identidade visual é original.
