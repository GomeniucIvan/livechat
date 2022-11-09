import $ from 'jQuery';
import { useEffect } from 'react';
import 'summernote';
import 'summernote/dist/summernote.css';
import { postLauncher } from '../../utils/HttpClient';
import Translate from '../../utils/Translate'
import { equal, isNullOrEmpty } from '../../utils/Utils';

const DetailsFooter = (props) => {
    let $summerNote = null;

    useEffect(() => {
        $summerNote = $('#reply-conv');

        $summerNote.summernote({
            tooltip: false,
            disableResizeEditor: true,
            disableDragAndDrop: true,
            shortcuts: false,
            toolbar: [
                ['style', ['style']],
                ['font', ['bold', 'underline', 'clear']],
                ['fontname', ['fontname']],
                ['color', ['color']],
                ['view', ['fullscreen']],
            ],
        }); 
        $('.note-statusbar').hide(); 

        $summerNote.on("summernote.change", function (e) {  
            const enteredCode = $summerNote.summernote('code');

            if (equal('<p><br></p>', enteredCode) || isNullOrEmpty(enteredCode)) {
                $('.btn-send').addClass('disabled');
            } else {
                $('.btn-send').removeClass('disabled');
            }
        });
    });

    const sendMessage = async () => {
        const enteredCode = $summerNote.summernote('code');
        const model = {
            Message: enteredCode
        };

        const result = await postLauncher('/api/sendText', model);
        if (result.IsValid) {
            $summerNote.summernote('reset');
            //$('.btn-send').addClass('disabled');
        }
    }

    return (
        <>
            <div className='conv-footer position-relative'>
                <div id='reply-conv'>

                </div>
                <button type='button' onClick={sendMessage} className='btn btn-primary position-absolute btn-send disabled'><Translate text="Send" /> </button>
            </div>
        </>
    )
}

export default DetailsFooter