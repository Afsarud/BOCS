(function () {
    // 1. পুরো ওয়েবপেজে রাইট-ক্লিক বন্ধ
    document.addEventListener('contextmenu', function (e) {
        e.preventDefault();
        return false;
    }, true);

    // 2. ভিডিও শেল / ফ্রেমে রাইট-ক্লিকও বন্ধ
    const videoShell = document.getElementById('videoShell');
    if (videoShell) {
        videoShell.addEventListener('contextmenu', function (e) {
            e.preventDefault();
            return false;
        }, true);
    }

    // 3. শর্টকাট কী ব্লক (Inspect, View Source, Save ইত্যাদি)
    document.addEventListener('keydown', function (e) {
        const k = (e.key || '').toLowerCase();
        const ctrl = e.ctrlKey || e.metaKey;
        if (k === 'f12' || (ctrl && (k === 'u' || k === 's')) ||
            (ctrl && e.shiftKey && (k === 'i' || k === 'j'))) {
            e.preventDefault();
            e.stopPropagation();
            return false;
        }
    }, true);

    // 4. Drag করে কিছু নেওয়া বন্ধ
    document.addEventListener('dragstart', function (e) {
        e.preventDefault();
        return false;
    }, true);
})();




//(function () {
//    document.addEventListener('contextmenu', e => { e.preventDefault(); }, { capture: true });
//    document.addEventListener('keydown', function (e) {
//        const k = (e.key || '').toLowerCase();
//        const ctrl = e.ctrlKey || e.metaKey;
//        if (k === 'f12' || (ctrl && (k === 'u' || k === 's')) ||
//            (ctrl && e.shiftKey && (k === 'i' || k === 'j'))) {
//            e.preventDefault(); e.stopPropagation();
//        }
//    }, true);

//    // safety: stop all drags (esp. images)
//    document.addEventListener('dragstart', e => e.preventDefault(), true);
//})();

