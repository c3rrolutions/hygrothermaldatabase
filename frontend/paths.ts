export default {
  home: "/",
  legalNotice: "/legal-notice",
  dataProtectionInformation: "/data-protection-information",
  antiforgeryToken: "/antiforgery/token",
  userInfo: "/user-info",
  database: "/database",
  calorimetricData: "/data/calorimetric",
  calorimetricDatum(uuid: string) {
    return `/data/calorimetric/${encodeURIComponent(uuid)}`;
  },
  hygrothermalData: "/data/hygrothermal",
  hygrothermalDatum(uuid: string) {
    return `/data/hygrothermal/${encodeURIComponent(uuid)}`;
  },
  opticalData: "/data/optical",
  opticalDatum(uuid: string) {
    return `/data/optical/${encodeURIComponent(uuid)}`;
  },
  photovoltaicData: "/data/photovoltaic",
  photovoltaicDatum(uuid: string) {
    return `/data/photovoltaic/${encodeURIComponent(uuid)}`;
  },
  geometricData: "/data/geometric",
  geometricDatum(uuid: string) {
    return `/data/geometric/${encodeURIComponent(uuid)}`;
  },
  createData: "/data/create",
  uploadFile: "/upload-file",
  login: "/connect/login",
  logout: "/connect/logout",
  metabase: {
    component(uuid: string) {
      return new URL(`/components/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
    database(uuid: string) {
      return new URL(`/databases/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
    dataFormat(uuid: string) {
      return new URL(`/data-formats/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
    institution(uuid: string) {
      return new URL(`/institutions/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
    method(uuid: string) {
      return new URL(`/methods/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
    user(uuid: string) {
      return new URL(`/users/${encodeURIComponent(uuid)}`, process.env.NEXT_PUBLIC_METABASE_URL).href
    },
  },
};
