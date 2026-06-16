import { useRouter } from "next/router";
import LifeCycleData from "../../../components/data/lifeCycle/LifeCycleData";
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
      <LifeCycleData id={String(uuid)} />
    </Layout>
  );
}

export default Page;
