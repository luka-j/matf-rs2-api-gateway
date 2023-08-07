import { cn } from "@/utils/style-utils";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

interface IOuterCardProps {
  title: string;
  description: string;
  viewAllURL?: string;
  children: React.ReactNode;
}

const OuterCard = ({ title, description, viewAllURL, children }: IOuterCardProps) => {
  return (
    <Card className="mx-4">
      <CardHeader>
        <CardTitle className={cn(viewAllURL && "flex justify-between")}>
          {title}
          {viewAllURL && (
            <Button asChild variant="outline" className="-mt-1">
              <a href={viewAllURL}>View all</a>
            </Button>
          )}
        </CardTitle>
        <CardDescription>{description}</CardDescription>
      </CardHeader>

      <CardContent className="flex flex-col items-center justify-between gap-8 md:flex-row">
        {children}
      </CardContent>
    </Card>
  );
};

export default OuterCard;
