import { useFormik } from 'formik';
import * as Yup from 'yup';
import clsx from 'clsx';
import { useState } from 'react';
import { post } from '../../utils/HttpClient';
import $ from 'jquery';
import { isNullOrEmpty, showAndHide, T } from '../../utils/Utils';
import Thobber from '../../utils/Thobber';

//todo Yup -> T

const installSchema = Yup.object().shape({
    email: Yup.string()
        .email('Wrong email format')
        .min(3, 'Min 3 characters')
        .max(50, 'Max 3 characters')
        .required('Email is required'),
    password: Yup.string()
        .min(3, 'Min 3 characters')
        .max(50, 'Max 3 characters')
        .required('Password is required'),
    confirmPassword: Yup.string()
        .required('Confirm password is required')
        .oneOf([Yup.ref('password'), null], 'Passwords must match'),
    dbServer: Yup.string()
        .when("useRawConnectionString", {
            is: 'false',
            then: Yup.string().required("Server name is required")
        }),
    dbName: Yup.string()
            .when("useRawConnectionString", {
                is: 'false',
                then: Yup.string().required("Database name is required")
            })
})

const initialValues = {
    email: 'admin@store.com',
    password: '123456',
    confirmPassword: '123456',
    dbServer: '',
    dbName: '',
    dbUserId: '',
    dbPassword: '',
    dbRawConnectionString: '',
    useRawConnectionString: 'false',
    DbAuthType: 'windows',
    mediaStorage: 'fs'
}

const Install = () => {
    const [loading, setLoading] = useState(false);
    const [showThobber, setShowThobber] = useState(false);
    var progressIntervall;

    const checkProgress = async () => {
        try {
            const result = await post('/api/progress');
            if (result) {
                if (result.Completed) {
                    window.clearInterval(progressIntervall);
                    $("#install-progress").html(T('Public.Install.Restart'));

                    //todo
                    //await post('/api/finalize?restart=' + result.Success);
                    //window.location.reload();
                } else if (!isNullOrEmpty(result.ProgressMessage)) {
                    $("#install-progress").html(result.ProgressMessage);
                }
            }
        } catch (e) {
            $("#install-progress").html(T('Public.Install.Restart'));
        }
    }

    const formik = useFormik({
        initialValues,
        validationSchema: installSchema,
        onSubmit: async (values, { setStatus, setSubmitting }) => {
            setLoading(true);

            try {
                var postModel = {
                    AdminEmail: values.email,
                    AdminPassword: values.password,
                    ConfirmPassword: values.confirmPassword,
                    DbServer: values.dbServer,
                    DbName: values.dbName,
                    DbUserId: values.dbUserId,
                    DbPassword: values.dbPassword,
                    DbRawConnectionString: values.dbRawConnectionString,
                    UseRawConnectionString: values.useRawConnectionString === 'true',
                    DataProvider: 'sqlserver',
                    DbAuthType: values.dbAuthType,
                    MediaStorage: values.mediaStorage,
                    PrimaryLanguage: 'en-US'
                };

                setShowThobber(true);

                // start the checkprogress interval
                progressIntervall = window.setInterval(checkProgress, 1500);

                var result = await post('/api/install', postModel);
                $('body').addClass('overflow-hidden');
                if (result.Success) {
                    $("#install-progress").html(T('Public.Install.Restart'));
                    $('.spinner').remove();
                } else {
                    setShowThobber(false);
                    setStatus(result.message);
                    setSubmitting(false);
                    setLoading(false);
                    $('body').removeClass('overflow-hidden');
                }

            } catch (error) {
                setShowThobber(false);
                $('body').removeClass('overflow-hidden');
                setStatus('Wrong Credentials');
                setSubmitting(false);
                setLoading(false);
            }
        },
    });

    const onUseRawConnectionStringChange = (e) => {
        if (e.target.value === 'false') {
            showAndHide('#ConnectionInfoPanel', '#RawConnectionStringPanel', /*duration*/ 400);
        } else {
            showAndHide('#RawConnectionStringPanel', '#ConnectionInfoPanel', /*duration*/ 400);
        }
    }

    const onDbAuthTypeChange = (e) => {
        if (e.target.value === 'sqlserver') {
            $('#DbUserId').prop("disabled", false);
            $('#DbPassword').prop("disabled", false);
        } else {
            $('#DbUserId').prop("disabled", true);
            $('#DbPassword').prop("disabled", true);
        }
    }

    return (
        <>
            <div className="install-page">
                <header id="header">
                    <nav id="navbar" className="navbar navbar-dark navbar-expand-sm content p-0">
                        <div className="navbar-text h5 p-0 m-0 pl-1">
                            Installation
                        </div>

                        <div className="collapse navbar-collapse ml-auto">
                            <ul className="navbar-nav ml-auto">
                                <li className="nav-item">
                                    <button type='button' className="nav-link navbar-tool" title="Restart Installation" rel="nofollow">
                                        <i className="bi bi-arrow-clockwise navbar-icon"></i>
                                    </button>
                                </li>
                            </ul>
                        </div>
                    </nav>
                </header>

                <div id="content" className="content">
                    <div className="cph">
                        <form className="form-horizontal"
                            autoComplete="on"
                            id="install-form"
                            onSubmit={formik.handleSubmit}
                            noValidate>

                            <div className="install-panel">
                                <div className="install-content">
                                    <p className="fs-h6 text-muted mb-5">
                                        To complete this wizard, you must know some information regarding your database server ("connection string"). Please contact your ISP if necessary. If you're installing on a local machine or server, you might need information from your System Admin.
                                    </p>

                                    <fieldset className="mb-5">
                                        <div className="heading mb-3">
                                            <h2 className="heading-title text-muted">Information</h2>
                                        </div>
                                        <div className="form-group row">
                                            <label className="col-md-3 col-form-label">Admin Email</label>
                                            <div className="col-md-9">
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
                                                    autoComplete='off'
                                                />
                                                {formik.touched.email && formik.errors.email && (
                                                    <div className='fv-plugins-message-container'>
                                                        <div className='fv-help-block'>
                                                            <span role='alert'>{formik.errors.email}</span>
                                                        </div>
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                        <div className="form-group row">
                                            <label className="col-md-3 col-form-label">Password</label>
                                            <div className="col-md-9">
                                                <input
                                                    {...formik.getFieldProps('password')}
                                                    className={clsx(
                                                        'form-control',
                                                        { 'is-invalid': formik.touched.password && formik.errors.password },
                                                        {
                                                            'is-valid': formik.touched.password && !formik.errors.password,
                                                        }
                                                    )}
                                                    type='password'
                                                    autoComplete='off'
                                                />
                                                {formik.touched.password && formik.errors.password && (
                                                    <div className='fv-plugins-message-container'>
                                                        <div className='fv-help-block'>
                                                            <span role='alert'>{formik.errors.password}</span>
                                                        </div>
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                        <div className="form-group row">
                                            <label className="col-md-3 col-form-label">Confirm Password</label>
                                            <div className="col-md-9">
                                                <input
                                                    {...formik.getFieldProps('confirmPassword')}
                                                    className={clsx(
                                                        'form-control',
                                                        { 'is-invalid': formik.touched.confirmPassword && formik.errors.confirmPassword },
                                                        {
                                                            'is-valid': formik.touched.confirmPassword && !formik.errors.confirmPassword,
                                                        }
                                                    )}
                                                    type='password'
                                                    autoComplete='off'
                                                />
                                                {formik.touched.confirmPassword && formik.errors.confirmPassword && (
                                                    <div className='fv-plugins-message-container'>
                                                        <div className='fv-help-block'>
                                                            <span role='alert'>{formik.errors.confirmPassword}</span>
                                                        </div>
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                    </fieldset>

                                    <fieldset className="mb-5">
                                        <div className="heading mb-3">
                                            <h2 className="heading-title text-muted">Database information</h2>
                                        </div>
                                        <div className="form-group row">
                                            <label className="col-md-3 col-form-label">Database system</label>
                                            <div className="col-md-9">
                                                <select className="form-control form-control" id="DataProvider" name="DataProvider">
                                                    <option defaultValue="selected" value="sqlserver">Microsoft SQL Server (Express)</option>
                                                </select>
                                            </div>
                                        </div>

                                        <div id="ConnectionPanel">
                                            <div className="form-group row">
                                                <label className="col-md-3 col-form-label" htmlFor="UseRawConnectionString">Connection</label>
                                                <div className="col-md-9">
                                                    <select
                                                        {...formik.getFieldProps('useRawConnectionString')}
                                                        className={clsx(
                                                            'form-control',
                                                            { 'is-invalid': formik.touched.useRawConnectionString && formik.errors.useRawConnectionString },
                                                            {
                                                                'is-valid': formik.touched.useRawConnectionString && !formik.errors.useRawConnectionString,
                                                            }
                                                        )}
                                                        onChange={(e) => {
                                                            onUseRawConnectionStringChange(e);
                                                            formik.setFieldValue('useRawConnectionString', e.target.value)
                                                        }}>
                                                        <option value="false" defaultValue="selected">Enter connection values</option>
                                                        <option value="true">Enter raw connection string (advanced)</option>
                                                    </select>

                                                    {formik.touched.useRawConnectionString && formik.errors.useRawConnectionString && (
                                                        <div className='fv-plugins-message-container'>
                                                            <div className='fv-help-block'>
                                                                <span role='alert'>{formik.errors.useRawConnectionString}</span>
                                                            </div>
                                                        </div>
                                                    )}
                                                </div>
                                            </div>

                                            <div id="ConnectionInfoPanel">
                                                <div className="form-group row">
                                                    <label className="col-md-3 col-form-label" htmlFor="DbServer">Server</label>
                                                    <div className="col-md-9">
                                                        <div className="form-row">
                                                            <div className="col-6">
                                                                <div className="has-icon">
                                                                    <input
                                                                        {...formik.getFieldProps('dbServer')}
                                                                        className={clsx(
                                                                            'form-control',
                                                                            { 'is-invalid': formik.touched.dbServer && formik.errors.dbServer },
                                                                            {
                                                                                'is-valid': formik.touched.dbServer && !formik.errors.dbServer,
                                                                            }
                                                                        )}
                                                                        type='text'
                                                                        placeholder='Server name'
                                                                    />

                                                                    <span className="input-group-icon text-muted">
                                                                        <i className="bi bi-hdd-stack"></i>
                                                                    </span>
                                                                </div>
                                                                {formik.touched.dbServer && formik.errors.dbServer && (
                                                                    <div className='fv-plugins-message-container'>
                                                                        <div className='fv-help-block'>
                                                                            <span role='alert'>{formik.errors.dbServer}</span>
                                                                        </div>
                                                                    </div>
                                                                )}
                                                            </div>
                                                            <div className="col-6">
                                                                <div className="has-icon">
                                                                    <input
                                                                        {...formik.getFieldProps('dbName')}
                                                                        className={clsx(
                                                                            'form-control',
                                                                            { 'is-invalid': formik.touched.dbName && formik.errors.dbName },
                                                                            {
                                                                                'is-valid': formik.touched.dbName && !formik.errors.dbName,
                                                                            }
                                                                        )}
                                                                        type='text'
                                                                        placeholder='Database name'
                                                                    />
                                                                    <span className="input-group-icon text-muted">
                                                                        <i className="bi bi-server"></i>
                                                                    </span>
                                                                </div>
                                                                {formik.touched.dbName && formik.errors.dbName && (
                                                                    <div className='fv-plugins-message-container'>
                                                                        <div className='fv-help-block'>
                                                                            <span role='alert'>{formik.errors.dbName}</span>
                                                                        </div>
                                                                    </div>
                                                                )}
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div className="form-group row">
                                                    <label className="col-md-3 col-form-label">Authentication</label>
                                                    <div className="col-md-9">
                                                        <select
                                                            {...formik.getFieldProps('dbAuthType')}
                                                            className={clsx(
                                                                'form-control',
                                                                { 'is-invalid': formik.touched.dbAuthType && formik.errors.dbAuthType },
                                                                {
                                                                    'is-valid': formik.touched.dbAuthType && !formik.errors.dbAuthType,
                                                                }
                                                            )}
                                                            onChange={(e) => {
                                                                onDbAuthTypeChange(e);
                                                                formik.setFieldValue('dbAuthType', e.target.value)
                                                            }}>
                                                            <option value="sqlserver">SQL Server-Authentication</option>
                                                            <option value="windows">Windows-Authentication</option>
                                                        </select>
                                                    </div>
                                                </div>

                                                <div className="form-group row">
                                                    <div className="col-md-9 offset-md-3">
                                                        <div className="form-row">
                                                            <div className="col-6">
                                                                <div className="has-icon">
                                                                    <input
                                                                        {...formik.getFieldProps('dbUserId')}
                                                                        className={clsx(
                                                                            'form-control',
                                                                            { 'is-invalid': formik.touched.dbUserId && formik.errors.dbUserId },
                                                                            {
                                                                                'is-valid': formik.touched.dbUserId && !formik.errors.dbUserId,
                                                                            }
                                                                        )}
                                                                        type='text'
                                                                        id='DbUserId'
                                                                        placeholder='Username'
                                                                    />

                                                                    <span className="input-group-icon text-muted">
                                                                        <i className="bi bi-person"></i>
                                                                    </span>
                                                                </div>
                                                                {formik.touched.dbUserId && formik.errors.dbUserId && (
                                                                    <div className='fv-plugins-message-container'>
                                                                        <div className='fv-help-block'>
                                                                            <span role='alert'>{formik.errors.dbUserId}</span>
                                                                        </div>
                                                                    </div>
                                                                )}
                                                            </div>
                                                            <div className="col-6">
                                                                <div className="has-icon">
                                                                    <input
                                                                        {...formik.getFieldProps('dbPassword')}
                                                                        className={clsx(
                                                                            'form-control',
                                                                            { 'is-invalid': formik.touched.dbPassword && formik.errors.dbPassword },
                                                                            {
                                                                                'is-valid': formik.touched.dbPassword && !formik.errors.dbPassword,
                                                                            }
                                                                        )}
                                                                        type='text'
                                                                        id='DbPassword'
                                                                        placeholder='Password'
                                                                    />
                                                                    <span className="input-group-icon text-muted">
                                                                        <i className="bi bi-shield"></i>
                                                                    </span>
                                                                </div>
                                                                {formik.touched.dbPassword && formik.errors.dbPassword && (
                                                                    <div className='fv-plugins-message-container'>
                                                                        <div className='fv-help-block'>
                                                                            <span role='alert'>{formik.errors.dbPassword}</span>
                                                                        </div>
                                                                    </div>
                                                                )}
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="RawConnectionStringPanel" style={{ display: 'none' }}>
                                                <div className="form-group row">
                                                    <label className="col-md-3 col-form-label" htmlFor="dbRawConnectionString">Connection string</label>
                                                    <div className="col-md-9">
                                                        <textarea
                                                            {...formik.getFieldProps('dbRawConnectionString')}
                                                            className={clsx(
                                                                'form-control',
                                                                { 'is-invalid': formik.touched.dbRawConnectionString && formik.errors.dbRawConnectionString },
                                                                {
                                                                    'is-valid': formik.touched.dbRawConnectionString && !formik.errors.dbRawConnectionString,
                                                                }
                                                            )}
                                                            rows="3"
                                                        />
                                                        {formik.touched.dbRawConnectionString && formik.errors.dbRawConnectionString && (
                                                            <div className='fv-plugins-message-container'>
                                                                <div className='fv-help-block'>
                                                                    <span role='alert'>{formik.errors.dbRawConnectionString}</span>
                                                                </div>
                                                            </div>
                                                        )}

                                                        <small id="RawConnectionStringHelp" className="form-text text-muted mt-2">
                                                            Example:
                                                            <span data-for-provider="sqlserver">
                                                                Data Source=MyServerName; Initial Catalog=MyDatabaseName; Persist Security Info=True; User Id=MyUserName; Password=MyPassword
                                                            </span>
                                                            <span data-for-provider="mysql">
                                                                Server=MyServerName; Database=MyDatabaseName; Uid=root; Pwd=MyPassword
                                                            </span>
                                                            <br />
                                                            Find more info <a href="http://www.connectionstrings.com/" rel="nofollow" target="_blank">here</a>
                                                        </small>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </fieldset>

                                    <fieldset className="mb-5">
                                        <div className="heading mb-3">
                                            <h2 className="heading-title text-muted">Install options</h2>
                                        </div>
                                        <div className="form-group row">
                                            <label className="col-md-3 col-form-label" htmlFor="MediaStorage">Media storage</label>
                                            <div className="col-md-9">
                                                <select
                                                    {...formik.getFieldProps('mediaStorage')}
                                                    className={clsx(
                                                        'form-control',
                                                        { 'is-invalid': formik.touched.mediaStorage && formik.errors.mediaStorage },
                                                        {
                                                            'is-valid': formik.touched.mediaStorage && !formik.errors.mediaStorage,
                                                        }
                                                    )}
                                                    onChange={(e) => {
                                                        formik.setFieldValue('mediaStorage', e.target.value)
                                                    }}>
                                                    <option defaultValue="selected" value="fs">File system (recommended)</option>
                                                    <option value="db">Database</option>
                                                </select>
                                                <small className="form-text text-muted mt-2">
                                                    File system' provides the highest possible performance and stability and is ideally suited for production environment.
                                                </small>
                                            </div>
                                        </div>
                                    </fieldset>
                                    <div className="form-group row">
                                        <div className="col-md-9 offset-md-3">
                                            {formik.status &&
                                                <div id="messages" className='alert alert-danger'>
                                                    {formik.status}
                                                </div>
                                            }

                                            <button type='submit'
                                                className='btn btn-primary btn-lg btn-block fs-h4 font-weight-normal'
                                                disabled={formik.isSubmitting || !formik.isValid}>

                                                {!loading &&
                                                    <>
                                                        <i className="bi bi-download"></i>
                                                        <span>Install</span>
                                                    </>
                                                }
                                                {loading && (
                                                    <span className='indicator-progress' style={{ display: 'block' }}>
                                                        Please wait...
                                                        <span className='spinner-border spinner-border-sm align-middle ms-2'></span>
                                                    </span>
                                                )}
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </div>

            <Thobber show={showThobber} large="true" flex="true" text={T('Public.Install.Header')} />
            
            </>
    )
}

export default Install;