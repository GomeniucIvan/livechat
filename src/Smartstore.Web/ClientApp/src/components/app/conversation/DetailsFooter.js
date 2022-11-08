import { Editor, EditorState } from "draft-js";
import "draft-js/dist/Draft.css";
import { useRef, useState } from "react";

const DetailsFooter = (props) => {
    const [editorState, setEditorState] = useState(() =>
        EditorState.createEmpty()
    );

    const editor = useRef(null);

    function focusEditor() {
        editor.current.focus();
    }

    return (
        <>
            <div className='conv-footer'>
                <div
                    style={{ border: "1px solid black", minHeight: "6em", cursor: "text" }}
                    onClick={focusEditor}
                >
                    <Editor
                        ref={editor}
                        editorState={editorState}
                        onChange={setEditorState}
                        placeholder="Write something!"
                    />
                </div>
            </div>
        </>
    )
}

export default DetailsFooter