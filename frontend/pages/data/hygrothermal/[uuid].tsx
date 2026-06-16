import { useRouter } from "next/router";
import HygrothermalData from "../../../components/data/hygrothermal/HygrothermalData";
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
      <HygrothermalData id={String(uuid)} />
    </Layout>
  );
}

export default Page;
