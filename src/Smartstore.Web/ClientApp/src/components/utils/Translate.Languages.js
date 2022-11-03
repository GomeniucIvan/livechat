let lang = {
    initialized: false,
    version: 0,
    resources: []
}
export default lang


export const setResources = (resources) => {
    lang.initialized = true;
    lang.resources = resources;
}
