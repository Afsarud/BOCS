// ========== course-info.js ==========
function readJsonFromScript(id, fallback) {
    const el = document.getElementById(id);
    if (!el) return fallback;
    try { return JSON.parse(el.textContent || ''); } catch { return fallback; }
}
function toYtId(input) {
    if (!input) return "";
    input = String(input).trim();
    if (/^[A-Za-z0-9_-]{11}$/.test(input)) return input;
    let m = input.match(/[?&]v=([A-Za-z0-9_-]{11})/); if (m) return m[1];
    m = input.match(/youtu\.be\/([A-Za-z0-9_-]{11})/); if (m) return m[1];
    m = input.match(/embed\/([A-Za-z0-9_-]{11})/); if (m) return m[1];
    return "";
}

const INITIAL_ID_RAW = readJsonFromScript('INITIAL_ID', '');
const LIST_FROM_SCRIPT = readJsonFromScript('LESSON_IDS_DATA', []);
const IS_ADMIN = (document.getElementById('IS_ADMIN_FLAG')?.value === 'true');

const LESSON_IDS = (Array.isArray(LIST_FROM_SCRIPT) && LIST_FROM_SCRIPT.length)
    ? LIST_FROM_SCRIPT
    : Array.from(document.querySelectorAll('.lesson-item')).map(li => li.getAttribute('data-yt') || '');

let START_ID = toYtId(INITIAL_ID_RAW);
if (!START_ID) {
    const firstPlayable = document.querySelector('.lesson-item[data-canplay="true"]')
        || (IS_ADMIN ? document.querySelector('.lesson-item') : null);
    if (firstPlayable) {
        START_ID = toYtId(firstPlayable.getAttribute('data-yt') || "");
        firstPlayable.classList.add('active');
    }
}

let player = null;

function onYouTubeIframeAPIReady() {
    const vid = START_ID || 'dQw4w9WgXcQ';
    player = new YT.Player('player', {
        videoId: vid,
        playerVars: {
            modestbranding: 1,
            rel: 0,
            controls: 0,
            disablekb: 1,
            iv_load_policy: 3,
            fs: 0,
            playsinline: 1
        },
        events: {
            onReady: (e) => { try { e.target.playVideo(); } catch { } }
        }
    });
}
window.onYouTubeIframeAPIReady = onYouTubeIframeAPIReady;

function idByIndex(idx) {
    const i = parseInt(idx, 10);
    if (Number.isNaN(i) || i < 0 || i >= LESSON_IDS.length) return "";
    return LESSON_IDS[i] || "";
}
document.addEventListener('click', function (e) {
    const li = e.target.closest('.lesson-item');
    if (!li) return;
    const allowed = (li.dataset.canplay === 'true') || IS_ADMIN;
    if (!allowed) { alert('Class not permitted.'); return; }

    const direct = li.getAttribute('data-yt');
    const byIdx = idByIndex(li.getAttribute('data-idx'));
    const ytId = toYtId(direct || byIdx);
    if (!ytId) return;

    document.querySelectorAll('.lesson-item.active').forEach(x => x.classList.remove('active'));
    li.classList.add('active');

    const subject = li.getAttribute('data-subject') || "";
    const lesson = li.getAttribute('data-lesson') || "";
    const titleEl = document.getElementById('lessonTitle');
    if (titleEl) titleEl.textContent = subject && lesson ? `${subject} | ${lesson}` : "";

    if (player && typeof player.loadVideoById === 'function') {
        player.loadVideoById(ytId);
        try { player.playVideo(); } catch { }
    } else {
        START_ID = ytId;
    }
});

document.addEventListener('keydown', function (e) {
    if ((e.key === 'Enter' || e.key === ' ') && document.activeElement?.classList.contains('lesson-item')) {
        e.preventDefault();
        document.activeElement.click();
    }
});

// ==== Custom Controls ====
document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("customPlay")?.addEventListener("click", () => player?.playVideo());
    document.getElementById("customPause")?.addEventListener("click", () => player?.pauseVideo());

    document.getElementById("volumeUp")?.addEventListener("click", () => {
        if (player) {
            let v = player.getVolume();
            player.setVolume(Math.min(v + 10, 100));
        }
    });
    document.getElementById("volumeDown")?.addEventListener("click", () => {
        if (player) {
            let v = player.getVolume();
            player.setVolume(Math.max(v - 10, 0));
        }
    });
    document.getElementById("muteToggle")?.addEventListener("click", () => {
        if (!player) return;
        if (player.isMuted()) player.unMute(); else player.mute();
    });

    document.getElementById("seekForward")?.addEventListener("click", () => {
        if (player) {
            let t = player.getCurrentTime();
            player.seekTo(t + 10, true);
        }
    });
    document.getElementById("seekBackward")?.addEventListener("click", () => {
        if (player) {
            let t = player.getCurrentTime();
            player.seekTo(Math.max(t - 10, 0), true);
        }
    });
});

