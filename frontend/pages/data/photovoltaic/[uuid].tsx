import { useRouter } from "next/router";
import PhotovoltaicData from "../../../components/data/photovoltaic/PhotovoltaicData";
import Layout from "../../../components/Layout";
import { Skeleton } from "antd";

function Page() {
  const router = useRouter();
  const { uuid } = router.query;

  if (!uuid) {
    return (
      <Layout>
        <Skeleton active avatar title />
      </Layout>
    );
  }

  return (
    <Layout>
      <PhotovoltaicData id={String(uuid)} />
    </Layout>
  );
}

export default Page;
