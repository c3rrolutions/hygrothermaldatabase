import { Scalars } from "../../../__generated__/__types__";
import { usePhotovoltaicDataQuery } from "../../../queries/data.graphql";
import { Skeleton, Result } from "antd";
import { useEffect } from "react";
import { messageApolloError } from "../../../lib/apollo";
import DataPageHeader from "../DataPageHeader";

export type PhotovoltaicDataProps = {
  photovoltaicDataId: Scalars["Uuid"];
};

export default function PhotovoltaicData({ photovoltaicDataId }: PhotovoltaicDataProps) {
  const { loading, error, data } = usePhotovoltaicDataQuery({
    variables: {
      uuid: photovoltaicDataId,
    },
  });
  const photovoltaicData = data?.photovoltaicData;

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!photovoltaicData) {
    return (
      <Result
        status="500"
        title="500"
        subTitle="Sorry, something went wrong."
      />
    );
  }

  return (
    <DataPageHeader
      data={photovoltaicData}
    // extra={[
    //   <UpdatePhotovoltaicData
    //     key="updatePhotovoltaicData"
    //     photovoltaicDataId={photovoltaicData.uuid}
    //   />,
    // ]}
    >
    </DataPageHeader>
  );
}
