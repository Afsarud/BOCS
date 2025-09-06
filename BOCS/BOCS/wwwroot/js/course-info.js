// YouTube প্লেয়ার বদলানো + indexing + delegation
(function () {
    const container = document.getElementById('videoTop');
    const LESSON_IDS = Array.isArray(window.__LESSON_IDS__) ? window.__LESSON_IDS__ : [];

    function toYtId(input) {
        if (!input) return "";
        input = String(input).trim();
        if (/^[A-Za-z0-9_-]{11}$/.test(input)) return input;
        let m = input.match(/[?&]v=([A-Za-z0-9_-]{11})/); if (m) return m[1];
        m = input.match(/youtu\.be\/([A-Za-z0-9_-]{11})/); if (m) return m[1];
        m = input.match(/embed\/([A-Za-z0-9_-]{11})/); if (m) return m[1];
        return "";
    }

    function idByIndex(idx) {
        const i = parseInt(idx, 10);
        if (Number.isNaN(i) || i < 0 || i >= LESSON_IDS.length) return "";
        return LESSON_IDS[i] || "";
    }

//    function renderPlayer(id) {
//        const yt = toYtId(id);
//        if (!yt || !container) return;

//        const bust = Date.now();
//        //const params = new URLSearchParams({
//        //    rel: '0',
//        //    controls: '0',        // ⬅️ gear/YouTube/Share bar লুকবে
//        //    modestbranding: '1',
//        //    fs: '0',              // fullscreen বাটন hide
//        //    disablekb: '1',       // কিবোর্ড শর্টকাট অফ
//        //    iv_load_policy: '3',  // annotations off
//        //    cc_load_policy: '0',  // captions default off
//        //    playsinline: '1',
//        //    autoplay: '1',
//        //    origin: location.origin,
//        //    enablejsapi: '1'
//        //});

//        container.innerHTML = `
//<iframe id="ytPlayer"
//  src="https://www.youtube-nocookie.com/embed/${yt}?${params.toString()}&_=${bust}"
//  title="Class player" frameborder="0"
//  allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
//  allowfullscreen></iframe>`;
//        container.scrollIntoView({ behavior: 'smooth', block: 'start' });
//    }


    function renderPlayer(id) {
        const yt = toYtId(id);
        if (!yt || !container) return;

        const params = new URLSearchParams({
            rel: '0', controls: '1', modestbranding: '1',
            fs: '0', disablekb: '0', iv_load_policy: '3', cc_load_policy: '0',
            playsinline: '1', autoplay: '1', mute: '1', origin: location.origin, enablejsapi: '1'
        });

        container.innerHTML = `
  <iframe id="ytPlayer"
    src="https://www.youtube-nocookie.com/embed/${yt}?${params.toString()}&_=${Date.now()}"
    title="Class player" frameborder="0"
    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
    sandbox="allow-scripts allow-same-origin allow-presentation"
    allowfullscreen></iframe>`;
    }

    // ক্লিক: পুরো পেজে delegation (accordion-এর ভিতরেও কাজ করবে)
    document.addEventListener('click', function (e) {
        const li = e.target.closest('.lesson-item');
        if (!li) return;

        const direct = li.getAttribute('data-yt');
        const byIdx = idByIndex(li.getAttribute('data-idx'));
        const id = direct || byIdx;
        if (!id) return;

        document.querySelectorAll('.lesson-item.active').forEach(x => x.classList.remove('active'));
        li.classList.add('active');

        renderPlayer(id);
    });

    // Keyboard support
    document.addEventListener('keydown', function (e) {
        if ((e.key === 'Enter' || e.key === ' ') && document.activeElement?.classList.contains('lesson-item')) {
            e.preventDefault();
            document.activeElement.click();
        }
    });

    // শুরুর সময়ে iframe না থাকলে index=0 প্লে korbe
    if (!document.getElementById('ytPlayer') && LESSON_IDS.length > 0) {
        const first = document.querySelector('.lesson-item[data-idx="0"]');
        if (first) {
            first.classList.add('active');
            renderPlayer(LESSON_IDS[0]);
        }
    }

})();