var quillEditor = new Quill("#full_editor", {
    bounds: "#full-container .editor",
    modules: {
        toolbar: [
            [{ font: [] }, { size: [] }],
            ["bold", "italic", "underline", "strike"],
            [
                { color: [] },
                { background: [] }
            ],
            [
                { script: "super" },
                { script: "sub" }
            ],
            [
                { list: "ordered" },
                { list: "bullet" },
                { indent: "-1" },
                { indent: "+1" }
            ],
            ["direction", { align: [] }],
            ["link", "image", "video"],
            ["clean"]]
    },
    theme: "snow"
});
/*quill.on('text-change', function () {
    var longDes = document.getElementById("AddLongDescription").value = quill.root.innerHTML;
});*/