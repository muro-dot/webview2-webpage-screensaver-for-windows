"""Self-contained offline HTML, with no external fonts or network dependencies."""
import json


def clock_html(language='ko'):
    return '''<!doctype html><html><meta charset="utf-8"><style>
    *{box-sizing:border-box}html,body{margin:0;width:100%;height:100%;overflow:hidden}
    body{display:grid;place-items:center;background:radial-gradient(ellipse at 50% 40%,#19263d,#0c101a 70%);color:#edf2fc;font-family:Segoe UI,Malgun Gothic,sans-serif;cursor:none}
    main{text-align:center}small{letter-spacing:.24em;color:#89a6c9;font-size:13px}
    h1{font-size:clamp(48px,13vw,180px);font-weight:300;letter-spacing:-.05em;margin:18px 0;text-shadow:0 0 70px #508dff40}
    p{color:#98a8bf;font-size:clamp(13px,2vw,22px)}
    </style><main><small>WEB SCREENSAVER</small><h1 id="clock"></h1><p id="date"></p></main><script>
    const locale=''' + json.dumps('ko-KR' if language == 'ko' else 'en-US') + ''';
    function tick(){let d=new Date();document.getElementById('clock').textContent=d.toLocaleTimeString('en-GB');document.getElementById('date').textContent=d.toLocaleDateString(locale,{weekday:'long',year:'numeric',month:'long',day:'numeric'});}tick();setInterval(tick,1000);
    </script></html>'''


def overlay_script(position):
    vertical = 'top' if position.startswith('Top') else 'bottom'
    horizontal = 'left' if position.endswith('Left') else 'right'
    return '''(()=>{if(document.getElementById('__ws_clock'))return;let e=document.createElement('div');e.id='__ws_clock';
    e.style.cssText="position:fixed;''' + vertical + ':24px;' + horizontal + ''':24px;z-index:2147483647;padding:12px 18px;border-radius:12px;background:#101622b8;color:white;font:24px Segoe UI,sans-serif;pointer-events:none;backdrop-filter:blur(12px)";
    document.documentElement.appendChild(e);function tick(){e.textContent=new Date().toLocaleTimeString('en-GB')}tick();setInterval(tick,1000)})()'''
