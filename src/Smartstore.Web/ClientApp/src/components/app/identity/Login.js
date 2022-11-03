import { useState } from 'react';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import clsx from 'clsx';
import Translate from '../../utils/Translate'
import { T } from '../../utils/Utils'
import { post } from '../../utils/HttpClient';

const loginSchema = Yup.object().shape({
    email: Yup.string()
        .email(T("App.Login.WrongEmailFormat"))
        .min(3, T("App.Login.MinmailSymbols"))
        .max(50, T("App.Login.MaxEmailSymbols"))
        .required(T("App.Login.EmailRequired")),
    password: Yup.string()
        .min(3, T("App.Login.MinPasswordSymbols"))
        .max(50, T("App.Login.MaxPasswordSymbols"))
        .required(T("App.Login.PasswordRequired")),
})

const initialValues = {
    email: '',
    password: '',
}

const Login = (props) => {
    const [loading, setLoading] = useState(false);

    const formik = useFormik({
        initialValues,
        validationSchema: loginSchema,
        onSubmit: async (values, { setStatus, setSubmitting }) => {
            setLoading(true);

            try {
                var postModel = {
                    Email: values.email,
                    Password: values.password
                };

                var result = await post('/api/login', postModel);
                if (result.IsValid) {
                    props.onLogin();
                } else {
                    setStatus(result.message);
                    setSubmitting(false);
                    setLoading(false);
                }

            } catch (error) {
                setStatus(T("App.Login.WrongCredentials"));
                setSubmitting(false);
                setLoading(false);
            }
        },
    });

    return (
        <div className="d-flex flex-column flex-root">
            <div className='d-flex flex-column flex-column-fluid'>

                <div className='d-flex flex-center flex-column flex-column-fluid p-10 pb-lg-20'>
                    <a href='#' className='mb-12'>
                        {/*<img alt='Logo' src={toAbsoluteUrl('/media/logos/logo-1.svg')} className='h-45px' />*/}
                    </a>

                    <div className='w-lg-500px bg-body rounded shadow-sm p-10 p-lg-15 mx-auto'>
                        <form
                            className='form w-100'
                            onSubmit={formik.handleSubmit}
                            noValidate>

                            <div className='text-center mb-10'>
                                <h1 className='text-dark mb-3'><Translate text="App.Login.Header" /></h1>
                            </div>

                            {formik.status &&
                                <div className='mb-lg-15 alert alert-danger'>
                                    <div className='alert-text font-weight-bold'>{formik.status}</div>
                                </div>
                            }

                            <div className="form-group row">
                                <div className="col-12">
                                    <div className="form-row">
                                        <div className="col-12 mb-4">
                                            <label className='form-label'>
                                                <Translate text="App.Login.Email" />
                                            </label>

                                            <div className="input-group input-group-lg has-icon">
                                                <input
                                                    {...formik.getFieldProps('email')}
                                                    className={clsx(
                                                        'form-control',
                                                        { 'is-invalid': formik.touched.email && formik.errors.email },
                                                        {
                                                            'is-valid': formik.touched.email && !formik.errors.email,
                                                        }
                                                    )}
                                                    type='email'
                                                    name='email'
                                                    autoComplete='off'
                                                />
                                                <span className="input-group-icon text-muted">
                                                    <i className="bi bi-person"></i>
                                                </span>
                                            </div>
                                            {formik.touched.email && formik.errors.email && (
                                                <div className='fv-plugins-message-container'>
                                                    <span role='alert'>{formik.errors.email}</span>
                                                </div>
                                            )}
                                        </div>

                                        <div className="col-12">
                                            <label className='form-label'>
                                                <Translate text="App.Login.Password" />
                                            </label>

                                            <div className="input-group input-group-lg has-icon">
                                                <input
                                                    type='password'
                                                    autoComplete='off'
                                                    {...formik.getFieldProps('password')}
                                                    className={clsx(
                                                        'form-control',
                                                        {
                                                            'is-invalid': formik.touched.password && formik.errors.password,
                                                        },
                                                        {
                                                            'is-valid': formik.touched.password && !formik.errors.password,
                                                        }
                                                    )}
                                                />
                                                <span className="input-group-icon text-muted">
                                                    <i className="bi bi-shield"></i>
                                                </span>
                                            </div>
                                            {formik.touched.password && formik.errors.password && (
                                                <div className='fv-plugins-message-container'>
                                                    <div className='fv-help-block'>
                                                        <span role='alert'>{formik.errors.password}</span>
                                                    </div>
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div className='text-center'>
                                <button
                                    type='submit'
                                    className='btn btn-lg btn-primary w-100 mb-5'
                                    disabled={formik.isSubmitting || !formik.isValid}
                                >
                                    {!loading && <span className='indicator-label'>Continue</span>}
                                    {loading && (
                                        <span className='indicator-progress' style={{ display: 'block' }}>
                                            Please wait...
                                            <span className='spinner-border spinner-border-sm align-middle ms-2'></span>
                                        </span>
                                    )}
                                </button>

                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default Login