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
      return `${
        process.env.NEXT_PUBLIC_METABASE_URL
      }/components/${encodeURIComponent(uuid)}`;
    },
    database(uuid: string) {
      return `/databases/${encodeURIComponent(uuid)}`;
    },
    dataFormat(uuid: string) {
      return `${
        process.env.NEXT_PUBLIC_METABASE_URL
      }/data-formats/${encodeURIComponent(uuid)}`;
    },
    institution(uuid: string) {
      return `${
        process.env.NEXT_PUBLIC_METABASE_URL
      }/institutions/${encodeURIComponent(uuid)}`;
    },
    method(uuid: string) {
      return `${
        process.env.NEXT_PUBLIC_METABASE_URL
      }/methods/${encodeURIComponent(uuid)}`;
    },
  },
};
