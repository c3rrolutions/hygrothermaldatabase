import { Scalars } from "../../../__generated__/graphql";
import { LifeCycleDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result, App } from "antd";
import { useEffect } from "react";
import { stringifyApolloError } from "../../../lib/apollo";
import DataPageHeader from "../DataPageHeader";
import { useQuery } from "@apollo/client/react";

export type LifeCycleDataProps = {
  lifeCycleDataId: Scalars["Uuid"]["input"];
};

export default function LifeCycleData({ lifeCycleDataId }: LifeCycleDataProps) {
  const { loading, error, data } = useQuery(LifeCycleDataDocument, {
    variables: {
      uuid: lifeCycleDataId,
    },
  });
  const lifeCycleData = data?.lifeCycleData;
  const { message } = App.useApp();

  useEffect(() => {
    if (error) {
      message.error(stringifyApolloError(error));
    }
  }, [error]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!lifeCycleData) {
    return (
      <Result
        status="500"
        title="500"
        subTitle="Sorry, something went wrong."
      />
    );
  }

  return (
    <>
      <DataPageHeader
        data={lifeCycleData}
        // extra={[
        //   <UpdateLifeCycleData
        //     key="updateLifeCycleData"
        //     lifeCycleDataId={lifeCycleData.uuid}
        //   />,
        // ]}
      ></DataPageHeader>
    </>
  );
}
