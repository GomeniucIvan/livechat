import $ from 'jQuery';
import { useEffect } from 'react';
import 'summernote';
import 'summernote/dist/summernote.css';

const DetailsFooter = (props) => {
    useEffect(() => {
        const $summerNote = $('#reply-conv');

        $summerNote.summernote({
            tooltip: false,
            disableResizeEditor: true
        }); 
        $('.note-statusbar').hide(); 
    });

    return (
        <>
            <div className='conv-footer position-relative'>
                <div id='reply-conv'>

                </div>
            </div>
        </>
    )
}

export default DetailsFooter